using UnityEngine;

public class MathWallPearent : MonoBehaviour
{
    public MathWall[] walls;
    
    public void DeactivateWalls()
    {
        foreach (MathWall wall in walls)
        {
            if (wall != null && wall.gameObject.activeSelf) 
            {
                wall.gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        foreach (MathWall wall in walls)
        {
            if (wall != null && !wall.gameObject.activeSelf)
            {
                wall.gameObject.SetActive(true);
            }
        }
    }
}
