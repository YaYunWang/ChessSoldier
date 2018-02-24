using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NewBehaviourScript : MonoBehaviour {

    // Use this for initialization
  public   PlayableDirector playableDirector;

    void Start () {
       // if (playableDirector.gameObject.name == "CoffeeBot")
        {
            foreach (PlayableBinding bind in playableDirector.playableAsset.outputs)
            {
                if (bind.sourceObject == null) {
                    Debug.Log("bind obj null");
                }
               Object valObj = playableDirector.GetGenericBinding(bind.sourceObject);
                if (valObj == null) {
                    playableDirector.SetGenericBinding(bind.sourceObject,Camera.main.GetComponent<CinemachineBrain>());
                }
               Debug.Log(valObj);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.K)) {
            //Debug.Log(playableDirector.playableGraph);
            playableDirector.Play();
        }
	}
}
