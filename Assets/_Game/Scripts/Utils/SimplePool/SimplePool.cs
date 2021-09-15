using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Utils
{

	public class SimplePool<T> where T : class
	{
		private Stack<T> pool = null;
		public Func<T, T> CreateFunction;
		public Action<T> OnPush, OnPop;

		public T Blueprint { get; set; }

		public SimplePool(Func<T, T> CreateFunction = null, Action<T> OnPush = null, Action<T> OnPop = null)
		{
			pool = new Stack<T>();

			this.CreateFunction = CreateFunction;
			this.OnPush = OnPush;
			this.OnPop = OnPop;
		}

		public SimplePool(T blueprint, Func<T, T> CreateFunction = null, Action<T> OnPush = null,
			Action<T> OnPop = null)
			: this(CreateFunction, OnPush, OnPop)
		{
			Blueprint = blueprint;
		}

		public bool Populate(int count)
		{
			return Populate(Blueprint, count);
		}

		public bool Populate(T blueprint, int count)
		{
			if (count <= 0)
				return true;

			T obj = NewObject(blueprint);
			if (obj == null)
				return false;

			Push(obj);

			for (int i = 1; i < count; i++)
			{
				Push(NewObject(blueprint));
			}

			return true;
		}

		public T Pop()
		{
			T objToPop;

			if (pool.Count == 0)
			{
				objToPop = NewObject(Blueprint);
			}
			else
			{
				objToPop = pool.Pop();
				while (objToPop == null)
				{
					// Some objects in the pool might have been destroyed (maybe during a scene transition),
					// consider that case
					if (pool.Count > 0)
						objToPop = pool.Pop();
					else
					{
						objToPop = NewObject(Blueprint);
						break;
					}
				}
			}

			if (OnPop != null)
				OnPop(objToPop);

			return objToPop;
		}

		// Fetch multiple items at once from the pool
		public T[] Pop(int count)
		{
			if (count <= 0)
				return new T[0];

			T[] result = new T[count];
			for (int i = 0; i < count; i++)
				result[i] = Pop();

			return result;
		}

		// Pool an item
		public void Push(T obj)
		{
			if (obj == null) return;

			if (OnPush != null)
				OnPush(obj);

			pool.Push(obj);
		}

		// Pool multiple items at once
		public void Push(IEnumerable<T> objects)
		{
			if (objects == null) return;

			foreach (T obj in objects)
				Push(obj);
		}

		// Clear the pool
		public void Clear(bool destroyObjects = true)
		{
			if (destroyObjects)
			{
				// Destroy all the Objects in the pool
				foreach (T item in pool)
				{
					Object.Destroy(item as Object);
				}
			}

			pool.Clear();
		}

		// Create an instance of the blueprint and return it
		private T NewObject(T blueprint)
		{
			if (CreateFunction != null)
				return CreateFunction(blueprint);

			if (blueprint == null || !(blueprint is Object))
				return null;

			return Object.Instantiate(blueprint as Object) as T;
		}
	}
}