using UnityEngine;
using System.Collections;
using UnitySocketIO;
using UnitySocketIO.Events;

public class SocketTestWebGL : MonoBehaviour {

	public SocketIOController io;
	private SpriteRenderer sprite;

	void Start() {
		sprite = GetComponent<SpriteRenderer>();
		
		
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
			sprite.color = Color.blue;
        });
		
		io.On("register", (SocketIOEvent e) => {
            Debug.Log("CORA");
        });
		
		/*
		io.On("keyword", (SocketIOEvent e) => {
            Debug.Log("registered");
        });
		*/
    }

}
