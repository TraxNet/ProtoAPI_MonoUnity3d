using System;
using System.Threading;

namespace com.protapi.RestClient
{
	class RESTAsyncResult : IAsyncResult
	{
		ManualResetEvent m_Event;
		bool m_IsCompleted = false;
		object m_AsyncState = null;
		
		public RESTAsyncResult (ManualResetEvent finished_event, object state)
		{
			m_AsyncState = state;
			m_Event = finished_event;
			m_Event.Reset();
		}
		
		public void SetCompleted(){
			m_IsCompleted = true;
			m_Event.Set();	
		}

		#region IAsyncResult implementation
		public object AsyncState {
			get {
				return m_AsyncState;
			}
		}
	
		public System.Threading.WaitHandle AsyncWaitHandle {
			get {
				return m_Event;
			}
		}
	
		public bool CompletedSynchronously {
			get {
				return false;
			}
		}
	
		public bool IsCompleted {
			get {
				return m_IsCompleted;
			}
		}
		#endregion
	}
}

