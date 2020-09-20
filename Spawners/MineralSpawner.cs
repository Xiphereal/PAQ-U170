using UnityEngine;

public class MineralSpawner : Spawner
{
    private ParticleSystem dust;

    [SerializeField]
    private int spawnCapacity;
    [SerializeField]
    private float miningTimeInSeconds;
    [SerializeField]
    private float shrinkRate;

    private WaitForSeconds miningTime;
    public WaitForSeconds MiningTime
    {
        get { return miningTime; }
    }

    void Awake()
    {
        miningTime = new WaitForSeconds(miningTimeInSeconds);
        dust = GetComponentInChildren<ParticleSystem>();

        InitializeDustGameObject();

        void InitializeDustGameObject()
        {
            dust.transform.parent = null;
            dust.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public bool TrySpawn()
    {
        if (spawnCapacity > 1)
        {
            SpawnItem();
            return true;
        }
        else
        {
            SpawnItem();

            if (gameObject != null)
                Destroy(gameObject);

            return false;
        }
    }

    protected override void SpawnItem()
    {
        dust.Stop();

        PlayerMovement player = LevelManager.Instance.PlayerMovement;
        spawnPosition = player.transform.position + player.transform.forward;

        Instantiate(prefab, spawnPosition, Quaternion.identity);

        ReduceMineralVeinOres();

        void ReduceMineralVeinOres()
        {
            spawnCapacity--;

            transform.localScale /= shrinkRate;
            transform.position = new Vector3(transform.position.x, transform.position.y - shrinkRate / 4, transform.position.z);
        }
    }

    public void GenerateDust()
    {
        PlayerMovement player = LevelManager.Instance.PlayerMovement;
        dust.transform.position = player.transform.position + player.transform.forward;

        dust.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<MiningBehavior>().AbleToMine = true;
            other.GetComponent<MiningBehavior>().Mineral = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<MiningBehavior>().AbleToMine = false;
            other.GetComponent<MiningBehavior>().Mineral = null;
        }
    }
}
