using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

namespace Axie
{
	public class Grid : MonoBehaviour
	{
		public int mapSize = 0;
		public float hexRadius = 1;
		[SerializeField] private Tile tilePrefab;

		private Dictionary<string, Tile> grid = new Dictionary<string, Tile>();

		private CubeIndex[] directions =
			new CubeIndex[]
			{
				new CubeIndex(1, -1, 0),
				new CubeIndex(1, 0, -1),
				new CubeIndex(0, 1, -1),
				new CubeIndex(-1, 1, 0),
				new CubeIndex(-1, 0, 1),
				new CubeIndex(0, -1, 1)
			};

		#region Getters and Setters

		public Dictionary<string, Tile> Tiles => grid;

		#endregion

		#region Public Methods

		public Tile TileAt(CubeIndex index)
		{
			if (grid.ContainsKey(index.ToString()))
				return grid[index.ToString()];
			return null;
		}

		public Tile TileAt(int x, int y, int z)
		{
			return TileAt(new CubeIndex(x, y, z));
		}

		public Tile TileAt(int x, int z)
		{
			return TileAt(new CubeIndex(x, z));
		}

		public List<Tile> Neighbours(Tile tile)
		{
			List<Tile> ret = new List<Tile>();

			if (tile == null)
				return ret;

			CubeIndex o;

			for (int i = 0; i < directions.Length; i++)
			{
				o = tile.Index + directions[i];
				if (grid.ContainsKey(o.ToString()))
					ret.Add(grid[o.ToString()]);
			}

			return ret;
		}

		public List<Tile> Neighbours(CubeIndex index)
		{
			return Neighbours(TileAt(index));
		}

		public List<Tile> Neighbours(int x, int y, int z)
		{
			return Neighbours(TileAt(x, y, z));
		}

		public List<Tile> Neighbours(int x, int z)
		{
			return Neighbours(TileAt(x, z));
		}

		public List<Tile> TilesInRange(Tile center, int range)
		{
			List<Tile> ret = new List<Tile>();
			CubeIndex o;

			for (int dx = -range; dx <= range; dx++)
			{
				for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
				{
					o = new CubeIndex(dx, dy, -dx - dy) + center.Index;
					if (grid.ContainsKey(o.ToString()))
						ret.Add(grid[o.ToString()]);
				}
			}

			return ret;
		}

		public List<Tile> TilesInRange(CubeIndex index, int range)
		{
			return TilesInRange(TileAt(index), range);
		}

		public List<Tile> TilesInRange(int x, int y, int z, int range)
		{
			return TilesInRange(TileAt(x, y, z), range);
		}

		public List<Tile> TilesInRange(int x, int z, int range)
		{
			return TilesInRange(TileAt(x, z), range);
		}

		public List<Tile> TilesInBound(Tile center, int range)
		{
			if (range == 0)
				return new List<Tile>() { center };

			List<Tile> ret = new List<Tile>();
			CubeIndex o;

			for (int dx = -range; dx <= range; dx++)
			{
				for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
				{
					o = new CubeIndex(dx, dy, -dx - dy) + center.Index;
					if (Math.Abs(o.x) == range || Math.Abs(o.y) == range || Math.Abs(o.z) == range)
					{
						if (grid.ContainsKey(o.ToString()))
							ret.Add(grid[o.ToString()]);
					}
				}
			}

			return ret;
		}

		public List<Tile> TilesInBound(int x, int y, int z, int range)
		{
			return TilesInBound(TileAt(x, y, z), range);
		}

		public int Distance(CubeIndex a, CubeIndex b)
		{
			return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
		}

		public int Distance(Tile a, Tile b)
		{
			return Distance(a.Index, b.Index);
		}

		#endregion

		public void Init(int size)
		{
			Debug.Log("Generating hexagonal shaped grid...");

			Tile tile;
			Vector3 pos = Vector3.zero;
			mapSize = size;

			for (int q = -mapSize; q <= mapSize; q++)
			{
				int r1 = Mathf.Max(-mapSize, -q - mapSize);
				int r2 = Mathf.Min(mapSize, -q + mapSize);
				for (int r = r1; r <= r2; r++)
				{
					var cubeIndex = new CubeIndex(q, r, -q - r);
					if (grid.ContainsKey(cubeIndex.ToString())) continue;

					// pos.x = hexRadius * 3.0f / 2.0f * q;
					// pos.y = hexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);

					pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
					pos.y = hexRadius * 3.0f / 2.0f * r;

					tile = CreateHexGO(pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
					tile.Index = new CubeIndex(q, r, -q - r);
					tile.transform.localScale = Vector3.zero;
					tile.transform.DOScale(1f, 0.3f);
					grid.Add(tile.Index.ToString(), tile);
				}
			}
		}

		public void ExtendGrid(int increaseSize)
		{
			Init(mapSize + increaseSize);
		}

		public float GetMapHeight()
		{
			return (float)((mapSize - 1) * Math.Sqrt(3) * hexRadius);
		}

		private Tile CreateHexGO(Vector3 postion, string name)
		{
			// var go = new GameObject(name, typeof(Tile));
			var tile = Instantiate(tilePrefab, postion, Quaternion.identity, transform);
			return tile;
		}
	}
}