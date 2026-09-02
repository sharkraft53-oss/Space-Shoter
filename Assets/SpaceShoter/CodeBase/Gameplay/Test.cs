using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] private string n_Name;

    public string Name // Сократить public string Name => n_Name;
    {
        get
        {
            return n_Name;
        }

        set
        {
            n_Name = value;
        }
    }


    #region Unity Event
    void Start()
    {
        
    }
   

    void Update()
    {
        
    }
    #endregion
}
