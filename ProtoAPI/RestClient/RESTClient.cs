using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace com.protapi.RestClient
{
	public class RESTClient
	{
		string m_uri;
		string m_BasicUser = null, m_BasicPwd = null;
		
		public delegate void OnAsyncResult(int status, string response);
		
		public RESTClient (string uri)
		{
			m_uri = uri;
		}
		
		public void SetBasicAuthorizationCredential(string user, string pwd){
			m_BasicPwd = pwd;
			m_BasicUser = user;
		}
		
		void AddAuthorizationHeader(HttpWebRequest request){
			string authInfo = m_BasicUser + ":" + m_BasicPwd;
    		authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
    		request.Headers["Authorization"] = "Basic " + authInfo;
		}
		
		public void AsyncGet(string path, Dictionary<string, string> qparams, OnAsyncResult callback){
			string complete_uri = ConcatPaths(m_uri, path);
			complete_uri += BuildQueryString(qparams);
			
			HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(complete_uri);
			request.Accept = "application/json";
			request.Method = "GET";
			
			AddAuthorizationHeader(request);
			
			request.BeginGetResponse(delegate (IAsyncResult result){
					string body = "";
	      			try{
						HttpWebRequest _request = (HttpWebRequest) result.AsyncState;
	      				HttpWebResponse response = (HttpWebResponse) _request.EndGetResponse(result);
					
						using(StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8)){
							body = sr.ReadToEnd();
						}
						
						callback((int)response.StatusCode, body);
					} catch(WebException e){ // TODO: Elaborate exceptions
						callback((int)((HttpWebResponse)e.Response).StatusCode, ((HttpWebResponse)e.Response).StatusDescription);
					} catch{
						callback(400, null);
					}
					
					
					
				}, request);
			
		}
		
		string ConcatPaths(string basepath, string path){
			if(path.StartsWith("/")){
				if(basepath.EndsWith("/")){
					basepath = basepath.Substring(0, basepath.Length-1);	
				}
				
				return basepath+path;
			} else{
				if(!basepath.EndsWith("/")){
					basepath = basepath + "/";	
				}	
				
				return basepath+path;
			}
		}
		
		public void AsyncPost(string path, Dictionary<string, string> qparams, string json, OnAsyncResult callback){
			string complete_uri = ConcatPaths(m_uri, path);
			complete_uri += BuildQueryString(qparams);
			
			HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(complete_uri);
			request.Accept = "application/json";
			request.Method = "POST";
			
			AddAuthorizationHeader(request);
			
			byte[] buffer = Encoding.UTF8.GetBytes(json);
			
			using(Stream request_stream = request.GetRequestStream())
			{
				request_stream.Write(buffer, 0, buffer.Length);
			}
			
			request.BeginGetResponse(delegate (IAsyncResult result){
					string body = "";
	      			try{
						HttpWebRequest _request = (HttpWebRequest) result.AsyncState;
	      				HttpWebResponse response = (HttpWebResponse) _request.EndGetResponse(result);
					
						using(StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8)){
							body = sr.ReadToEnd();
						}
					
						callback((int)response.StatusCode, body);
					} catch(WebException e){ // TODO: Elaborate exceptions
						callback((int)((HttpWebResponse)e.Response).StatusCode, ((HttpWebResponse)e.Response).StatusDescription);
					} catch{
						callback(400, null);
					}
					
					
					
				}, request);
		}
		
		string BuildQueryString(Dictionary<string, string> qparams){
			if(null == qparams)
				return "";
			
			StringBuilder str = new StringBuilder("?");
			
			bool prev = false;
			foreach(KeyValuePair<string,string> pair in qparams){
				if(prev)
					str.Append("&");
				
				str.Append(pair.Key);
				str.Append("=");
				str.Append(pair.Value);
				
				prev = true;
			}
			
			return str.ToString();
		}
	}
}

