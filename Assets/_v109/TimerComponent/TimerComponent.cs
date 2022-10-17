using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Model;
using Utility;

public struct Timer
{
	public long Id { get; set; }
	public long Time { get; set; }
	public TaskCompletionSource<bool> tcs;
}

public class TimerComponent : SingletonMono<TimerComponent>
{
	private readonly Dictionary<long, Timer> timers = new Dictionary<long, Timer>();

	/// <summary>
	/// key: time, value: timer id
	/// </summary>
	private readonly MultiMap<long, long> timeId = new MultiMap<long, long>();

	private readonly List<long> timeOutId = new List<long>();
	
	// public static TimerComponent Instance;
	// // Use this for initialization
	// void Start ()
	// {
	// 	Instance = this;
	// }
	
	// Update is called once per frame
	void Update () {
		long timeNow = TimeHelper.Now();

		while (true)
		{
			if (this.timeId.Count <= 0)
			{
				return;
			}
			var kv = this.timeId.First();
			if (kv.Key > timeNow)
			{
				break;
			}


			timeOutId.Clear();
			timeOutId.AddRange(kv.Value);

			this.timeId.Remove(kv.Key);
				
			foreach (long id in timeOutId)
			{
				Timer timer;
				if (!this.timers.TryGetValue(id, out timer))
				{
					continue;
				}
				this.timers.Remove(id);
				timer.tcs.SetResult(true);
			}				
		}
	}
	
		private void Remove(long id)
		{
			Timer timer;
			if (!this.timers.TryGetValue(id, out timer))
			{
				return;
			}
			this.timers.Remove(id);
		}

		public Task WaitTillAsync(long tillTime, CancellationToken cancellationToken)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = tillTime, tcs = tcs };
			this.timers[timer.Id] = timer;
			this.timeId.Add(timer.Time, timer.Id);
			cancellationToken.Register(() => { this.Remove(timer.Id); });
			return tcs.Task;
		}

		public Task WaitTillAsync(long tillTime)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = tillTime, tcs = tcs };
			this.timers[timer.Id] = timer;
			this.timeId.Add(timer.Time, timer.Id);
			return tcs.Task;
		}

		public Task WaitAsync(long time, CancellationToken cancellationToken)
		{ 
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
			this.timers[timer.Id] = timer;
			this.timeId.Add(timer.Time, timer.Id);
			cancellationToken.Register(() => { this.Remove(timer.Id); });
		    cancellationToken.Register(() => { tcs.SetCanceled(); });
            return tcs.Task;
		}

		public Task WaitAsync(long time)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
			this.timers[timer.Id] = timer;
			this.timeId.Add(timer.Time, timer.Id);
			return tcs.Task;
		}

//		public Timer WaitAsyncReturnTimer(long time)
//		{
//			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
//			Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
//			this.timers[timer.Id] = timer;
//			this.timeId.Add(timer.Time, timer.Id);
//			return timer;
//		}

		public Timer WaitAsyncReturnTimer(long time, CancellationToken cancellationToken)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Timer timer = new Timer { Id = IdGenerater.GenerateId(), Time = TimeHelper.Now() + time, tcs = tcs };
			this.timers[timer.Id] = timer;
			this.timeId.Add(timer.Time, timer.Id);
			cancellationToken.Register(() => { this.Remove(timer.Id); });
			cancellationToken.Register(
				() =>
			{
//				Log.Error($"{cancellationToken.GetHashCode()} is doing registered cancel. timer id :{timer.Id}, time:{timer.Time}");
				tcs.TrySetCanceled();
			});


			return timer;
		}

		//		public void RemoveATimer(Timer timer)
		//		{
		//			this.Remove(timer.Id);
		//			timer.tcs.SetCanceled();
		//		}
	}

