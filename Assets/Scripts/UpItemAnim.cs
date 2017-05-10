﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant;

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
		ItemInfo to = null;

		if(character.state.Equals(CharState.change_item)) {
			to = character.item;
			character.PutDown (character.item);
			character.state = CharState.pick_up;

		}

		character.PickUp (character.currTile.item);
		character.fix = false;
		character.currTile.item = to;
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
