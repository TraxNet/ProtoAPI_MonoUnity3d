using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;

using com.protapi.RestClient;
using com.protapi.Exceptions;

namespace com.protapi
{
	public class ProtoAPI
	{
		public delegate void ProtoAPIGetCallback<T>(ResponseStatus<T> status, T ret_object);
		public delegate void ProtoAPIPostCallback(ResponseStatus<string> status, string id);
		
		#region Private members
		
		string m_username = null, m_appkey = null;
		string m_ServiceUri = "http://localhost:8888/1/";
		static ProtoAPI g_SharedInstance = null;
		static object g_SharedLock = new object();
		
		#endregion
		
		#region Public properties
		
		/// <summary>
		/// Sets the username which is your ProtoAPI's username 
		/// </summary>
		/// <value>
		/// The username for authorization purpouses
		/// </value>
		public string username{
			set{
				m_username = value;
			}
		}
		
		/// <summary>
		/// Sets the Application Key. The Pair username/appkey must be set prior making calls to ProtoAPI platform.
		/// </summary>
		/// <value>
		/// The Application Key. Use the 'Client' application key to perform calls from public side.
		/// </value>
		public string appkey{
			set{
				m_appkey = value;
			}
		}
		
		#endregion
		
		#region Public methods
		
		/// <summary>
		/// Initializes a new instance of the <see cref="com.protapi.ProtoAPI"/> class. 
		/// Use ProtoAPI.SharedInstance instead <see cref="com.protapi.ProtoAPI.SharedInstance"/>.
		/// </summary>
		public ProtoAPI ()
		{
		}
		
		/// <summary>
		/// Gets a global shared instance.
		/// </summary>
		/// <value>
		/// The shared instance.
		/// </value>
		static public ProtoAPI SharedInstance{
			get{
				lock(g_SharedLock){
					if(null == g_SharedInstance)
						g_SharedInstance = new ProtoAPI();
					
					return g_SharedInstance;
				}
			}
		}
		
		
		public IAsyncResult BeginGet<T>(string cls, string id, ProtoAPIGetCallback<T> callback){
			if(string.IsNullOrEmpty(cls) || string.IsNullOrEmpty(id)){
				throw new ProtoAPIException("Invalid parameters: AsyncGet cannot be completed");	
			}
			
			CheckUserAndAppKey();
			
			ResponseStatus<T> response_status = new ResponseStatus<T>();
			ManualResetEvent finished_event = new ManualResetEvent(false);
			RESTAsyncResult async_result = new RESTAsyncResult(finished_event, response_status);
			
			RESTClient client = new RESTClient(m_ServiceUri);
			client.SetBasicAuthorizationCredential(m_username, m_appkey);
		
			client.AsyncGet("objects/"+cls+"/"+id, null, delegate (int status, string response){
				response_status.statusCode = status;
				
				if(status >= 200 && status < 300){
					JsonResponseObject<T> data_wrapper;
					data_wrapper = JsonConvert.DeserializeObject<JsonResponseObject<T>>(response, new IsoDateTimeConverter());
					
					response_status.responseData = data_wrapper.data;
					
					if(null != callback)
						callback(response_status, data_wrapper.data);
				} else{
					response_status.responseData = 	response;
					response_status.errorMessage = response;
					
					if(null != callback)
						callback(response_status, default(T));
				}
				
				async_result.SetCompleted();
			});
			
			return async_result;
		}
		
		public T EndGet<T>(IAsyncResult result, int milisecs){
			result.AsyncWaitHandle.WaitOne(milisecs);	
			if(!result.IsCompleted)
				return default(T);
			
			ResponseStatus<T> response = (ResponseStatus<T>) result.AsyncState;
			return (T) response.responseData;
		}
		
		public IAsyncResult BeginPost(string classname, object pobject, ProtoAPIPostCallback callback){
			if(string.IsNullOrEmpty(classname) || null == pobject){
				throw new ProtoAPIException("Invalid parameters: AsyncGet cannot be completed");	
			}	
			
			CheckUserAndAppKey();
			
			ResponseStatus<string> response_status = new ResponseStatus<string>();
			ManualResetEvent finished_event = new ManualResetEvent(false);
			RESTAsyncResult async_result = new RESTAsyncResult(finished_event, response_status);
			
			string payload = null;
			try{
				payload = JsonConvert.SerializeObject(pobject, new IsoDateTimeConverter());
			} catch(Exception e){
				throw new ProtoAPIException("Unable to serialize object to json: " + e.Message);
			}
			if(string.IsNullOrEmpty(payload)){
				throw new ProtoAPIException("Resulting json string is null or empty");	
			}
			
			RESTClient client = new RESTClient(m_ServiceUri);
			client.SetBasicAuthorizationCredential(m_username, m_appkey);
			
			client.AsyncPost("objects/"+classname, null, payload, delegate (int status, string response){
				response_status.statusCode = status;
				
				if(status == 201){
					JsonResponseObject<string> data_wrapper;
					data_wrapper = JsonConvert.DeserializeObject<JsonResponseObject<string>>(response, new IsoDateTimeConverter());
					
					response_status.responseData = data_wrapper.data;
					
					if(null != callback)
						callback(response_status, data_wrapper.data as string);	
				
				} else{
					response_status.responseData = 	response;
					response_status.errorMessage = response;
					
					if(null != callback)
						callback(response_status, null);
				}
				
				async_result.SetCompleted();
				
				
			});
			
			return async_result;
		}
		
		#endregion
		
		#region Private methods
		
		void CheckUserAndAppKey()
		{
			if(string.IsNullOrEmpty(m_username) || string.IsNullOrEmpty(m_appkey)){
				throw new ProtoAPIException("Invalid username and/or application key");	
			}
		}
		
		#endregion
	}
}

