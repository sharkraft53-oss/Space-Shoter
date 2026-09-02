using UnityEngine;

/// <summary>
/// БАзовый класс всех интерактивных игровых объектов на сцене.
/// </summary>
public abstract class Entity : MonoBehaviour
{
    /// <summary>
    ///  Название объектов для пользователя.
    /// </summary>
    [SerializeField] private string m_Nickname;
    public string Nickname => m_Nickname;


}
