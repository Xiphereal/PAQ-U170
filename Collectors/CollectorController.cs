using System.Linq;
using UnityEngine;

public class CollectorController : MonoBehaviour
{
    private static bool[] fruitCollected = new bool[3];

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Fruit fruit = LevelManager.Instance.PlayerInteractions.CarriedObject as Fruit;

            if (fruit != null)
            {
                if (fruit.Name == "Fruit1" && gameObject.name.Contains("1"))
                {
                    fruit.ObjectPlaced();
                    fruitCollected[0] = true;
                }
                else if (fruit.Name == "Fruit2" && gameObject.name.Contains("2"))
                {
                    fruit.ObjectPlaced();
                    fruitCollected[1] = true;
                }
                else if (fruit.Name == "Fruit3" && gameObject.name.Contains("3"))
                {
                    fruit.ObjectPlaced();
                    fruitCollected[2] = true;
                }

                if (fruitCollected.ToList().All(collected => collected == true))
                    LevelManager.Instance.SecondTaskCompleted();
            }
        }
    }
}
