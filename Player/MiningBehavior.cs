using System.Collections;
using UnityEngine;

public class MiningBehavior : MonoBehaviour
{
    private Rigidbody playerRigidBody;
    private PlayerMovement playerMovement;

    public bool AbleToMine { get; set; }
    public MineralSpawner Mineral { get; set; }

    private bool isMining;

    void Start()
    {
        playerRigidBody = LevelManager.Instance.PlayerMovement.Rigidbody;
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        CheckForMineCommand();

        void CheckForMineCommand()
        {
            if (AbleToMine && Input.GetButtonDown("Fire1") && IsFocusingThisMineralVein() && !isMining)
            {
                isMining = true;
                Mineral.GenerateDust();
                StartCoroutine(Mining(Mineral.MiningTime));

                AudioManager.Instance.Play("Mining");
                LockPlayer();
            }

            bool IsFocusingThisMineralVein()
            {
                Item focusedItem = GetComponent<PlayerInteractions>().GetFocusedItem();

                if (focusedItem == null)
                    return false;

                return focusedItem.Equals(Mineral.GetComponent<NonPickableItem>());
            }
            IEnumerator Mining(WaitForSeconds waitTime)
            {
                yield return waitTime;

                isMining = false;

                AbleToMine = Mineral.TrySpawn();
                AudioManager.Instance.Play("MineralOre");
                UnlockPlayer();

                void UnlockPlayer()
                {
                    playerRigidBody.isKinematic = false;
                    playerMovement.IsAbleToRotate = true;
                }
            }
            void LockPlayer()
            {
                playerRigidBody.isKinematic = true;
                playerMovement.IsAbleToRotate = false;
            }
        }
    }
}
