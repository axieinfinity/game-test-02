using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Utils;
using Random = System.Random;

namespace Axie
{
    public class MainGameController : Singleton<MainGameController>
    {
        [SerializeField] private Grid grid;
        [SerializeField] private CameraMovement cameraMovement;
        [SerializeField] private Camera miniMapCam;
        [SerializeField] private PowerBar powerBar;

        private const float MIN_FPS = 30;

        private int radius = 0;
        private int maxRadius = 0;
        private int defenderMaxRadius;
        private List<AxieCharacter> attackers = new List<AxieCharacter>();
        private List<AxieCharacter> defenders = new List<AxieCharacter>();

        #region Init Grid State

        IEnumerator InitGrid()
        {
            grid.Init(1);
            
            var wait = new WaitForSeconds(Constants.GameLogic.TIME_INIT_MAP * 0.25f);
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

            maxRadius = radius * 2;
            defenderMaxRadius = radius;
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
            MessageUI.Instance.Show("Generating Largest Map ...", -1);
            yield return new WaitForSeconds(1f);
            yield return InitGrid();
            UpdateAttackersAndDefenders();
            Debug.LogError($"Total Axies: {grid.Tiles.Count(x => x.Value.IsEmpty == false)}");
            MessageUI.Instance.Show($"Generate Map Completed! Total Axies: {attackers.Count + defenders.Count}", 5);
            yield return GameLoop();
        }

        IEnumerator GameLoop()
        {
            var wait = new WaitForSeconds(Constants.GameLogic.TIME_IN_LOOP * 0.5f);
            
            while (true)
            {
                yield return wait;
                UpdateTiles();
                yield return wait;
                powerBar.UpdateBar(attackers, defenders);

                if (CheckEndGame())
                {
                    cameraMovement.EndGameAnim();
                    PlayWinAnim();
                    yield break;
                }
            }
        }

        private void PlayWinAnim()
        {
            foreach (var tile in grid.Tiles.Values)
            {
                if (tile.IsEmpty == false)
                {
                    tile.Axie.PlayWinAnim();
                }
            }
        }

        private bool CheckEndGame()
        {
            attackers.RemoveAll(x => x.gameObject.activeSelf == false);
            defenders.RemoveAll(x => x.gameObject.activeSelf == false);
            
            if (attackers.Sum(x => x.HP) <= 0)
            {
                UIController.Instance.ShowEndGameUI(AxieType.Defender);
                return true;
            }
            
            if (defenders.Sum(x => x.HP) <= 0)
            {
                UIController.Instance.ShowEndGameUI(AxieType.Attacker);
                return true;
            }

            return false;
        }

        public void UpdateTiles()
        {
            var result = new List<Tile>();
            var dictAttackers = new Dictionary<string, Tile>();
            var emptyTiles = new List<Tile>();
            
            UpdateMaxRadius();

            foreach (var tile in grid.Tiles.Values)
            {
                if (tile.Index.Radius() > maxRadius) continue;
                
                if (tile.IsEmpty)
                {
                    emptyTiles.Add(tile);
                }
                else if (tile.AxieType == AxieType.Defender && tile.Index.Radius() >= defenderMaxRadius)
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

        private void UpdateMaxRadius()
        {
            var tileInRadius = grid.TilesInBound(0, 0, 0, maxRadius);
            bool allEmpty = true;
            foreach (var tile in tileInRadius)
            {
                if (tile.IsEmpty == false)
                {
                    allEmpty = false;
                    break;
                }
            }

            if (allEmpty) maxRadius -= 1;

            tileInRadius = grid.TilesInBound(0, 0, 0, defenderMaxRadius);
            bool allDefender = true;
            foreach (var tile in tileInRadius)
            {
                if (tile.IsEmpty == true || tile.AxieType == AxieType.Attacker)
                {
                    allDefender = false;
                    break;
                }
            }

            if (!allDefender) defenderMaxRadius -= 1;
        }
        
        public bool CanMergeAxie(AxieCharacter axie)
        {
            if (axie.AxieType == AxieType.Attacker) return false;
            var neighbours = grid.Neighbours(axie.CubeIndex);
            foreach (var tile in neighbours)
            {
                if (tile.IsEmpty) return false;
                if (tile.AxieType == AxieType.Attacker) return false;
            }

            return true;
        }

        public void MergeAxie(AxieCharacter axie)
        {
            if (CanMergeAxie(axie) ==  false) return;
            var neighbours = grid.Neighbours(axie.CubeIndex);
            foreach (var tile in neighbours)
            {
                tile.Axie.transform.DOMove(axie.transform.position, 0.3f).OnComplete(() =>
                {
                    tile.ClearAxie();
                });
            }
            axie.Upgrade();
            UpdateAttackersAndDefenders();
        }

        public void ExplosiveAxise(AxieCharacter axie)
        {
            if ((float)axie.HP/axie.MaxHP > Constants.GameLogic.LOW_HP) return;
            
            var neighbours = grid.TilesInRange(axie.CubeIndex, axie.Level);
            foreach (var tile in neighbours)
            {
                if (tile.IsEmpty == false && tile.AxieType != axie.AxieType)
                {
                    tile.Axie.TakeDame(axie.HP * 2);
                }
            }
            axie.Explosive();
        }
    }
}
