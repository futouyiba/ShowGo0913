﻿using System.Collections.Generic;
using System.Linq;

namespace Model
{
	public class MultiMap<T, K>
	{
		private readonly SortedDictionary<T, List<K>> dictionary = new SortedDictionary<T, List<K>>();

		// 重用list
		private readonly Queue<List<K>> queue = new Queue<List<K>>();

		public SortedDictionary<T, List<K>> GetDictionary()
		{
			return this.dictionary;
		}

		public void Add(T t, K k)
		{
			List<K> list;
			this.dictionary.TryGetValue(t, out list);
			if (list == null)
			{
				list = this.FetchList();
			}
			list.Add(k);
			this.dictionary[t] = list;
		}

		public void AddRange(T t, List<K> listOfKs)
		{
			List<K> list;
			this.dictionary.TryGetValue(t, out list);
			if (list == null)
			{
				list = this.FetchList();
			}
			list.AddRange(listOfKs);
			this.dictionary[t] = list;
		}

		public KeyValuePair<T, List<K>> First()
		{
			return this.dictionary.First();
		}

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		private List<K> FetchList()
		{
			if (this.queue.Count > 0)
			{
				List<K> list = this.queue.Dequeue();
				list.Clear();
				return list;
			}
			return new List<K>();
		}

		private void RecycleList(List<K> list)
		{
			// 防止暴涨
			if (this.queue.Count > 100)
			{
				return;
			}
			list.Clear();
			this.queue.Enqueue(list);
		}

		public bool Remove(T t, K k)
		{
			List<K> list;
			this.dictionary.TryGetValue(t, out list);
			if (list == null)
			{
				return false;
			}
			if (!list.Remove(k))
			{
				return false;
			}
			if (list.Count == 0)
			{
				this.RecycleList(list);
				this.dictionary.Remove(t);
			}
			return true;
		}

		public bool Remove(T t)
		{
			List<K> list = null;
			this.dictionary.TryGetValue(t, out list);
			if (list != null)
			{
				this.RecycleList(list);
			}
			return this.dictionary.Remove(t);
		}

		/// <summary>
		/// 不返回内部的list,copy一份出来
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public K[] GetAll(T t)
		{
			List<K> list;
			this.dictionary.TryGetValue(t, out list);
			if (list == null)
			{
				return new K[0];
			}
			return list.ToArray();
		}

		/// <summary>
		/// 返回内部的list
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public List<K> this[T t]
		{
			get
			{
				List<K> list;
				this.dictionary.TryGetValue(t, out list);
				return list;
			}
		}

		public K GetOne(T t)
		{
			List<K> list;
			this.dictionary.TryGetValue(t, out list);
			if (list != null && list.Count > 0)
			{
				return list[0];
			}
			return default(K);
		}

		public bool Contains(T t, K k)
		{
			List<K> list;
			this.dictionary.TryGetValue(t, out list);
			if (list == null)
			{
				return false;
			}
			return list.Contains(k);
		}

		public void Clear()
		{
			this.dictionary.Clear();
		}

		/// <summary>
		/// 加这个的作用是为了:来一个matchmaking请求,我看看这个哥们现在在不在匹配池子里
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public object ContainsInWhichType(K k)
		{
			foreach (KeyValuePair<T,List<K>> keyValuePair in dictionary)
			{
				if (keyValuePair.Value.Contains(k))
				{
					return keyValuePair.Key;
				}
			}

			return null;
		}

		
		/// <summary>
		/// 这是一个用来+
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public bool RemoveIfExist(K k)
		{
			foreach (KeyValuePair<T,List<K>> keyValuePair in dictionary)
			{
				var value = keyValuePair.Value;
				if (value.Contains(k))
				{
					value.Remove(k);
					return true;
				}
			}

			return false;
		}
	}
}