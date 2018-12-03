using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnitySocketIO.SocketIO;
using UnitySocketIO.Events;


namespace UnitySocketIO {
    public class Network : MonoBehaviour {
        
		//Socket Initialization
        public SocketIOSettings settings;
        BaseSocketIO socket;
        public string SocketID { get { return socket.SocketID; } }

        private void Awake() {
            if(Application.platform == RuntimePlatform.WebGLPlayer) {
                socket = gameObject.AddComponent<WebGLSocketIO>();
            }
            else {
                socket = gameObject.AddComponent<NativeSocketIO>();
            }
            
            socket.Init(settings);
        }
		
		//Prefab to spawn other players
		public GameObject playerPrefab;
		
		//Network Initialization
		private void Start() {
			socket.Connect();

			socket.On("register", OnRegister);
			socket.On("spawn", OnSpawned);
			
		}
		
		//Method to confirm connection
		private void OnRegister (SocketIOEvent e)
		{
			Debug.Log ("Succesfully registered player " + e.data);
			//socket.Emit("move");
		}
			
		//Method to spawn another player
		private void OnSpawned (SocketIOEvent e)
		{
			Debug.Log ("Another player spawned " + e.data);
			Instantiate (playerPrefab);
		}
		
		
		//Move the Character for other people
		public static void SendMove(float x, float y) {
			Debug.Log("sending position to node" + DirectionsToJson(x,y));
			//Send position to server
			//socket.Emit("move", DirectionsToJson(x,y));
		}
		
		//method for sending position to sockets
		public static string DirectionsToJson(float x, float y){
			return string.Format(@"{{""x"":""{0}"", ""y"":""{1}""}}", x, y);
		}
		
		
		
		
		
		//TODO: Delete below comments
		//FOR REFERENCE ONLY//
		/*
		void Start() {
		
		
        io.On("connect", (SocketIOEvent e) => {
            Debug.Log("SocketIO connected");
            io.Emit("test-event1");

            TestObject t = new TestObject();
            t.test = 123;
            t.test2 = "test1";

            io.Emit("test-event2", JsonUtility.ToJson(t));

            TestObject t2 = new TestObject();
            t2.test = 1234;
            t2.test2 = "test2";

            io.Emit("test-event3", JsonUtility.ToJson(t2), (string data) => {
                Debug.Log(data);
            });

        });

        io.Connect();

        io.On("test-event", (SocketIOEvent e) => {
            Debug.Log(e.data);
        });
		}
		*/
    



    }
}