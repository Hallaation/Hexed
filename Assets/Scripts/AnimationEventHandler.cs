using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
    Animator MyAnimator;
    Move MyMoveScript;
	// Use this for initialization
	void Start () {
        MyAnimator = GetComponent<Animator>();
        transform.root.GetComponent<Move>();
        MyMoveScript = transform.root.GetComponent<Move>();
	}
	public void FrameCompare()
    {
#if UNITY_EDITOR
        Animator OtherAnimator = MyMoveScript.chokingPlayer.transform.root.GetComponent<Move>().GetBodyAnimator();
        AnimatorStateInfo OtherCurrentState = OtherAnimator.GetCurrentAnimatorStateInfo(0);
        float OtherplaybackTime = OtherCurrentState.normalizedTime % 1;

        AnimatorStateInfo MyCurrentState = MyAnimator.GetCurrentAnimatorStateInfo(0);
        float MyPlaybackTime = MyCurrentState.normalizedTime % 1;
        
#endif
    }
	// Update is called once per frame
	void Update () {
		
	}
}
