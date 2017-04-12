﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpItemAnim : StateMachineBehaviour {
	private CharMove character;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//		Debug.Log ("up item enter");
		character = animator.gameObject.GetComponent<CharMove> ();
		
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//		Debug.Log ("up item exit");

		if(character.state.Equals(CharMove.CharState.change_item)) {
			character.PutDown (character.item);
			character.state = CharMove.CharState.pick_up;

		}

		character.PickUp (character.currTarget);
		character.fix = false;
	}

	//OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//	
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
//	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//		Debug.Log ("onstateik");
//	}
}