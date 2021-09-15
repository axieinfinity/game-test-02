using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace Utils
{
	public class SkeletonAnimationStateHandler : MonoBehaviour
	{
		public SkeletonAnimation skeletonAnimation;
		public List<StateNameToAnimationReference> statesAndAnimations = new List<StateNameToAnimationReference>();

		public List<AnimationTransition>
			transitions =
				new List<AnimationTransition>(); // Alternately, an AnimationPair-Animation Dictionary (commented out) can be used for more efficient lookups.

		[System.Serializable]
		public class StateNameToAnimationReference
		{
			public string stateName;
			public AnimationReferenceAsset animation;
		}

		[System.Serializable]
		public class AnimationTransition
		{
			public AnimationReferenceAsset from;
			public AnimationReferenceAsset to;
			public AnimationReferenceAsset transition;
		}

		//readonly Dictionary<Spine.AnimationStateData.AnimationPair, Spine.Animation> transitionDictionary = new Dictionary<AnimationStateData.AnimationPair, Animation>(Spine.AnimationStateData.AnimationPairComparer.Instance);

		public Spine.Animation TargetAnimation { get; private set; }

		void Awake()
		{
			// Initialize AnimationReferenceAssets
			foreach (var entry in statesAndAnimations)
			{
				entry.animation.Initialize();
			}

			foreach (var entry in transitions)
			{
				entry.from.Initialize();
				entry.to.Initialize();
				entry.transition.Initialize();
			}

			// Build Dictionary
			//foreach (var entry in transitions) {
			//	transitionDictionary.Add(new AnimationStateData.AnimationPair(entry.from.Animation, entry.to.Animation), entry.transition.Animation);
			//}
		}

		/// <summary>Sets the horizontal flip state of the skeleton based on a nonzero float. If negative, the skeleton is flipped. If positive, the skeleton is not flipped.</summary>
		public void SetFlip(float horizontal)
		{
			if (horizontal != 0)
			{
				skeletonAnimation.Skeleton.FlipX = horizontal > 0;
			}
		}

		public void SetFlip(bool isFacingRight)
		{
			skeletonAnimation.Skeleton.FlipX = isFacingRight;
		}

		/// <summary>Plays an animation based on the state name.</summary>
		public void PlayAnimationForState(string stateShortName, int layerIndex, bool loop = true)
		{
			PlayAnimationForState(StringToHash(stateShortName), layerIndex, loop);
		}

		public void ClearTrack(int layerIndex)
		{
			skeletonAnimation.state.ClearTrack(layerIndex);
		}

		/// <summary>Plays an animation based on the hash of the state name.</summary>
		public void PlayAnimationForState(int shortNameHash, int layerIndex, bool loop = true)
		{
			var foundAnimation = GetAnimationForState(shortNameHash);
			if (foundAnimation == null)
				return;

			PlayNewAnimation(foundAnimation, layerIndex, loop);
		}

		public void PlayOneShot(string shortNameHash, int layerIndex)
		{
			var foundAnimation = GetAnimationForState(shortNameHash);
			if (foundAnimation == null)
				return;
			var state = skeletonAnimation.AnimationState;
			state.AddAnimation(layerIndex, foundAnimation, false, 0);
			state.AddAnimation(0, this.TargetAnimation, true, 0f);
		}

		/// <summary>Gets a Spine Animation based on the state name.</summary>
		public Spine.Animation GetAnimationForState(string stateShortName)
		{
			return GetAnimationForState(StringToHash(stateShortName));
		}

		/// <summary>Gets a Spine Animation based on the hash of the state name.</summary>
		public Spine.Animation GetAnimationForState(int shortNameHash)
		{
			foreach (var state in statesAndAnimations)
			{
				if (StringToHash(state.stateName) == shortNameHash)
					return state.animation;
			}
			return null;
		}

		private Spine.Animation lastLoopAnim = null;
		
		/// <summary>Play an animation. If a transition animation is defined, the transition is played before the target animation being passed.</summary>
		public void PlayNewAnimation(Spine.Animation target, int layerIndex, bool loop = true)
		{
			Spine.Animation transition = null;
			Spine.Animation current = null;

			current = GetCurrentAnimation(layerIndex);
			if (current != null)
				transition = TryGetTransition(current, target);

			if (transition != null)
			{
				skeletonAnimation.AnimationState.SetAnimation(layerIndex, transition, false);
				skeletonAnimation.AnimationState.AddAnimation(layerIndex, target, loop, 0f);
			}
			else
			{
				skeletonAnimation.AnimationState.SetAnimation(layerIndex, target, loop);
			}

			this.TargetAnimation = target;
		}

		/// <summary>Play a non-looping animation once then continue playing the state animation.</summary>
		public void PlayOneShot(Spine.Animation oneShot, int layerIndex)
		{
			var state = skeletonAnimation.AnimationState;
			state.SetAnimation(0, oneShot, false);

			var transition = TryGetTransition(oneShot, TargetAnimation);
			if (transition != null)
				state.AddAnimation(0, transition, false, 0f);

			state.AddAnimation(0, this.TargetAnimation, true, 0f);
		}

		Spine.Animation TryGetTransition(Spine.Animation from, Spine.Animation to)
		{
			foreach (var transition in transitions)
			{
				if (transition.from.Animation == from && transition.to.Animation == to)
				{
					return transition.transition.Animation;
				}
			}

			return null;

			//Spine.Animation foundTransition = null;
			//transitionDictionary.TryGetValue(new AnimationStateData.AnimationPair(from, to), out foundTransition);
			//return foundTransition;
		}

		Spine.Animation GetCurrentAnimation(int layerIndex)
		{
			var currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(layerIndex);
			return (currentTrackEntry != null) ? currentTrackEntry.Animation : null;
		}

		int StringToHash(string s)
		{
			return Animator.StringToHash(s);
		}
	}
}