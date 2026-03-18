using UnityEngine;

public class PruebaIK : MonoBehaviour
{
    [Header("Variables asignables")]

    [SerializeField] private Transform handRTarget;
    [SerializeField] private Transform handLTarget;
    [SerializeField] private Transform footRTarget;
    [SerializeField] private Transform footLTarget;
    [SerializeField] private Transform elbowR;
    [SerializeField] private Transform elbowL;

    [SerializeField] private float weightIK;

    [SerializeField] private Transform kneeR;
    [SerializeField] private Transform kneeL;
    [SerializeField] private Transform footR;
    [SerializeField] private Transform footL;

    private Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (handRTarget != null)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, handRTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, handRTarget.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weightIK);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weightIK);

            animator.SetIKHintPosition(AvatarIKHint.RightElbow, elbowR.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, weightIK);
        }

        if (handLTarget != null)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, handLTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, handLTarget.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weightIK);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weightIK);

            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, elbowL.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, weightIK);
        }

        if (footRTarget != null)
        {
            animator.SetIKPosition(AvatarIKGoal.RightFoot, footRTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, footRTarget.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, weightIK);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, weightIK);

            animator.SetIKHintPosition(AvatarIKHint.RightKnee, kneeR.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, weightIK);
        }

        if (footLTarget != null)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, footLTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, footLTarget.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weightIK);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, weightIK);

            animator.SetIKHintPosition(AvatarIKHint.LeftKnee, kneeL.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, weightIK);
        }
    }
}
