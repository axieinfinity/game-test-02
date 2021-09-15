using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Axie
{
    public class MainGameController : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private CameraMovement cameraMovement;
        [SerializeField] private Camera miniMapCam;
        [SerializeField] private PowerBar powerBar;

        private const float MIN_FPS = 30;

        private int radius = 0;
        private List<AxieCharacter> attackers = new List<AxieCharacter>();
        private List<AxieCharacter> defenders = new List<AxieCharacter>();

        #region Init Grid State

        IEnumerator InitGrid()
        {
            grid.Init(1);
            
            var wait = new WaitForSeconds(0.25f);
            while (FPSCounter.Instance.FPS >= MIN_FPS)
            {
                yield return wait;
                grid.ExtendGrid(2);
                yield return wait;
                
                if (radius == 0)
                {
                    SetupAxie(AxieType.Defender, radius);
                    radius = 1;
                }
                
                SetupAxie(AxieType.Defender, radius);
                ClearAxise(radius + 1);
                for (int i = 0; i <= radius; i++)
                {
                    SetupAxie(AxieType.Attacker, radius + i + 2);    
                }

                radius += 1;
                var mapHeight = grid.GetMapHeight();
                cameraMovement.UpdateBound(mapHeight);
                miniMapCam.orthographicSize = mapHeight + 3;
                
                UpdateAttackersAndDefenders();
                yield return wait;
                yield return wait;
            }
            
            Debug.LogError($"Total : {attackers.Count + defenders.Count}");
            Debug.LogError($"Total Axies: {grid.Tiles.Count(x => x.Value.IsEmpty == false)}");
        }

        private void UpdateAttackersAndDefenders()
        {
            attackers.Clear();
            defenders.Clear();
            foreach (var tile in grid.Tiles.Values)
            {
                if (tile.Axie != null)
                {
                    if (tile.Axie.AxieType == AxieType.Attacker)
                        attackers.Add(tile.Axie);
                    else
                        defenders.Add(tile.Axie);
                }
            }
            powerBar.UpdateBar(attackers, defenders);
        }

        void SetupAxie(AxieType axieType, int bound)
        {
            var tiles = grid.TilesInBound(0, 0, 0, bound);
            foreach (var tile in tiles)
            {
                tile.SetAxie(axieType);
            }
        }

        void ClearAxise(int bound)
        {
            var tiles = grid.TilesInBound(0, 0, 0, bound);
            foreach (var tile in tiles)
            { 
                tile.ClearAxie();
            }
        }
        
        #endregion
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            MessageUI.Instance.Show("Generating Largest Map ...", 10);
            yield return new WaitForSeconds(1f);
            yield return InitGrid();
            MessageUI.Instance.Show($"Generate Map Completed! Total Axies: {attackers.Count + defenders.Count}", 5);
            yield return GameLoop();
        }

        IEnumerator GameLoop()
        {
            var wait = new WaitForSeconds(1f);
            
            while (true)
            {
                yield return wait;
                UpdateTiles();
                yield return wait;
                powerBar.UpdateBar(attackers, defenders);
            }
        }

        public void UpdateTiles()
        {
            var result = new List<Tile>();
            var dictAttackers = new Dictionary<string, Tile>();
            var emptyTiles = new List<Tile>();
            
            foreach (var tile in grid.Tiles.Values)
            {
                if (tile.IsEmpty)
                {
                    emptyTiles.Add(tile);
                }
                else if (tile.AxieType == AxieType.Defender)
                {
                    var attackerNeighbours = grid.Neighbours(tile)
                        .Where(x => x.IsEmpty == false && x.AxieType == AxieType.Attacker);
                    foreach (var attacker in attackerNeighbours)
                    {
                        if (dictAttackers.ContainsKey(attacker.Index.ToString()) == false)
                        {
                            dictAttackers.Add(attacker.Index.ToString(), attacker);
                            result.Add(attacker);
                            attacker.AttackAxie(tile);
                        }
                    }
                }
            }

            foreach (var tile in emptyTiles)
            {
                var attackerNeighbours = grid.Neighbours(tile)
                    .Where(x => x.IsEmpty == false && x.AxieType == AxieType.Attacker && x.Index >= tile.Index && !dictAttackers.ContainsKey(x.Index.ToString())).ToList();
                if (attackerNeighbours.Count > 0)
                {
                    var rand = UnityEngine.Random.Range(0, attackerNeighbours.Count);
                    result.Add(attackerNeighbours[rand]);
                    attackerNeighbours[rand].MoveAxieTo(tile);
                }
            }
        }
    }
}
