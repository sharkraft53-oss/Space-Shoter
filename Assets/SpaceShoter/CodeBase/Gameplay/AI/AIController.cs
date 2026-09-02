using JetBrains.Annotations;
using SpaceShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpaceShip))]
public class AIController : MonoBehaviour
{
    public enum AIBehavior
    {
        Null,
        Patrol,
        PatrolRoute
    }

    [SerializeField] private AIBehavior m_AIBehavior;

    [SerializeField] private AIPointPatrol m_PatrolPoint;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_NavigationLinear;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_NavigationAngular;

    [SerializeField] private float m_RandomSelectMovePointTime;

    [SerializeField] private float m_FindNewTargetTime;

    [SerializeField] private float m_ShootDelay;

    [SerializeField] private float m_EvadeRayLength;

    [SerializeField] private AIPointPatrol[] m_PatrolRoute;

    [SerializeField] private float m_PatrolPointRadius = 1.0f;

    [SerializeField] private float m_ProjectileSpeed = 20f;

    private int m_CurrentPatrolIndex = 0;

    private SpaceShip m_SpaceShip;

    private Vector3 m_MovePosition;

    private Vector3 m_LeadPoint;

    private Destructible m_SelectedTarget;

    private Timer m_RandomizeDirectionTimer;
    private Timer m_FireTimer;
    private Timer m_FindNewTargetTimer;

    [SerializeField] private int m_TeamId;
    public int TeamId => m_TeamId;

    private void Start()
    {
        m_SpaceShip = GetComponent<SpaceShip>();

        InitTimers();
    }

    private void Update()
    {
        UpdateTimers();

        UpdateAI();

    }

    private void UpdateAI()
    {
        if (m_AIBehavior == AIBehavior.Patrol)
        {
            UpdateBehaviourPatrol();
        }

        if (m_AIBehavior == AIBehavior.PatrolRoute)
        {
            UpdateBehaviourPatrol();
        }
    }

    private void UpdateBehaviourPatrol()
    {
        ActionFindNewMovePosition();
        ActionControlShip();
        ActionFindNewAttackTarget();
        ActionFire();
        ActionAvadeCollision();
    }


    private void ActionFindNewMovePosition()
    {


        if (m_AIBehavior == AIBehavior.Patrol)
        {
            if (m_SelectedTarget != null)
            {
                Rigidbody2D targetRb = m_SelectedTarget.GetComponent<Rigidbody2D>();

                if (targetRb != null)
                {
                    Vector2 leadPos = MakeLead(m_SpaceShip.transform.position,
                                               m_SelectedTarget.transform.position,
                                               targetRb.linearVelocity,
                                               m_ProjectileSpeed);
                    m_MovePosition = leadPos;
                    m_LeadPoint = leadPos;
                }
                else
                {
                    m_MovePosition = m_SelectedTarget.transform.position;
                    m_LeadPoint = m_SelectedTarget.transform.position;
                }
            }
            else
            {
                if (m_PatrolPoint != null)
                {
                    bool isInsidePatrolZone = (m_PatrolPoint.transform.position - transform.position).sqrMagnitude < (m_PatrolPoint.Radius * m_PatrolPoint.Radius);

                    if (isInsidePatrolZone == true)
                    {
                        if (m_RandomizeDirectionTimer.IsFinished == true)
                        {
                            Vector2 newPoint = UnityEngine.Random.onUnitSphere * m_PatrolPoint.Radius + m_PatrolPoint.transform.position;

                            m_MovePosition = newPoint;

                            m_RandomizeDirectionTimer.Start(m_RandomSelectMovePointTime);
                        }
                    }
                    else
                    {
                        m_MovePosition = m_PatrolPoint.transform.position;
                    }
                }
            }
        }

        if (m_AIBehavior == AIBehavior.PatrolRoute)
        {
            if (m_SelectedTarget != null && IsTargetInPatrolRouteZone(m_SelectedTarget.transform.position))
            {
                Rigidbody2D targetRb = m_SelectedTarget.GetComponent<Rigidbody2D>();

                if (targetRb != null)
                {
                    Vector2 leadPos = MakeLead(m_SpaceShip.transform.position,
                                               m_SelectedTarget.transform.position,
                                               targetRb.linearVelocity,
                                               m_ProjectileSpeed);
                    m_MovePosition = leadPos;
                    m_LeadPoint = leadPos;
                }
                else
                {
                    m_MovePosition = m_SelectedTarget.transform.position;
                    m_LeadPoint = m_SelectedTarget.transform.position;
                }
            }
            else
            {
                // Если цель не в зоне или ее нет, сбрасываем цель и продолжаем патрулирование
                if (m_SelectedTarget != null && !IsTargetInPatrolRouteZone(m_SelectedTarget.transform.position))
                {
                    m_SelectedTarget = null;
                }

                if (m_PatrolRoute != null && m_PatrolRoute.Length > 0)
                {
                    Vector3 currentPoint = m_PatrolRoute[m_CurrentPatrolIndex].transform.position;
                    float distanceSqr = (currentPoint - transform.position).sqrMagnitude;

                    if (distanceSqr < m_PatrolPointRadius * m_PatrolPointRadius)
                    {
                        m_CurrentPatrolIndex = (m_CurrentPatrolIndex + 1) % m_PatrolRoute.Length;
                        currentPoint = m_PatrolRoute[m_CurrentPatrolIndex].transform.position;
                    }

                    m_MovePosition = currentPoint;
                }
            }
        }
    }

    private bool IsTargetInPatrolRouteZone(Vector3 targetPosition)
    {
        if (m_PatrolRoute == null || m_PatrolRoute.Length < 3)
            return false;

        Vector2 p = targetPosition;

        bool inside = false;

        // Перебираем рёбра полигона (каждое ребро от точки i до j)
        for (int i = 0, j = m_PatrolRoute.Length - 1; i < m_PatrolRoute.Length; j = i++)
        {
            // Точки текущего ребра
            Vector2 pi = m_PatrolRoute[i].transform.position;
            Vector2 pj = m_PatrolRoute[j].transform.position;

            // Проверяем, пересекает ли горизонтальный луч вправо из точки p это ребро
            bool intersectY = (pi.y > p.y) != (pj.y > p.y);

            if (intersectY)
            {
                // Находим точку пересечения луча с ребром
                float xCross = (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x;

                // Если точка P левее пересечения, значит луч пересёк ребро
                if (p.x < xCross)
                {
                    inside = !inside;
                }
            }
        }

        return inside;
    }

    /*
    private bool IsTargetInPatrolRouteZone(Vector3 targetPosition)
    {
        if (m_PatrolRoute == null) return false;

        foreach (AIPointPatrol point in m_PatrolRoute)
        {
            if (point != null)
            {
                float distanceSqr = (point.transform.position - targetPosition).sqrMagnitude;
                if (distanceSqr < point.Radius * point.Radius)
                {
                    return true;
                }
            }
        }
        return false;
    }
    */
    private void ActionAvadeCollision()
    {
        float rayDistance = m_EvadeRayLength;

        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, transform.up, rayDistance);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 90) * transform.up, rayDistance);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -90) * transform.up, rayDistance);

        if (forwardHit.collider != null)
        {
            if (leftHit.collider == null)
            {
                m_MovePosition = transform.position + (Quaternion.Euler(0, 0, 90) * transform.up) * 100.0f;
            }
            else if (rightHit.collider == null)
            {
                m_MovePosition = transform.position + (Quaternion.Euler(0, 0, -90) * transform.up) * 100.0f;
            }
            else
            {
                m_MovePosition = transform.position - transform.up * 100.0f;
            }

            m_SpaceShip.TorqueControl = 1.0f;
        }
    }

    /// <summary>
    /// Вычисляет точку упреждения для движущейся цели.
    /// </summary>
    /// <param name="shooterPos">позиция AI корабля</param>
    /// <param name="targetPos">позиция цели</param>
    /// <param name="targetVel">скорость цели</param>
    /// <param name="projectileSpeed">скорость снаряда</param>
    private Vector2 MakeLead(Vector2 shooterPos, Vector2 targetPos, Vector2 targetVel, float projectileSpeed)
    {
        Vector2 toTarget = targetPos - shooterPos;

        // Rоэффициенты квадратного уравнения
        float a = Vector2.Dot(targetVel, targetVel) - projectileSpeed * projectileSpeed;
        float b = 2 * Vector2.Dot(toTarget, targetVel);
        float c = Vector2.Dot(toTarget, toTarget);

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0f || Mathf.Abs(a) < 0.001f)
        {
            return targetPos;
        }

        // квадратное уравнение для времени встречи (t)
        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);

        // Минимальное положительное время
        float t = Mathf.Min(t1, t2);
        if (t < 0f) t = Mathf.Max(t1, t2);
        if (t < 0f) return targetPos;


        // Вычисление позицию цели через t секунд
        return targetPos + targetVel * t;
    }

    private void ActionControlShip()
    {
        m_SpaceShip.ThrustControl = m_NavigationLinear;

        m_SpaceShip.TorqueControl = ComputeAliginTorqueNormalized(m_MovePosition, m_SpaceShip.transform) * m_NavigationAngular;
    }

    private const float MAX_ANGLE = 45.0f;

    private static float ComputeAliginTorqueNormalized(Vector3 targetPosition, Transform ship)
    {
        Vector2 localTargetPosition = ship.InverseTransformPoint(targetPosition);

        float angle = Vector3.SignedAngle(localTargetPosition, Vector3.up, Vector3.forward);

        angle = Mathf.Clamp(angle, -MAX_ANGLE, MAX_ANGLE) / MAX_ANGLE;

        return -angle;
    }

    private void ActionFindNewAttackTarget()
    {
        if (m_FindNewTargetTimer.IsFinished == true)
        {
            Destructible newTarget = FindNearestDestructibleTarget();

            if (m_AIBehavior == AIBehavior.PatrolRoute && newTarget != null)
            {
                if (!IsTargetInPatrolRouteZone(newTarget.transform.position))
                    newTarget = null;
            }

            m_SelectedTarget = FindNearestDestructibleTarget();

            m_FindNewTargetTimer.Start(m_FindNewTargetTime);
        }
    }
    private void ActionFire()
    {
        if (m_SelectedTarget != null)
        {
            if (m_FireTimer.IsFinished == true)
            {
                Rigidbody2D targetRb = m_SelectedTarget.GetComponent<Rigidbody2D>();
                Vector2 aimPos = m_SelectedTarget.transform.position;

                if (targetRb != null)
                {
                    aimPos = MakeLead(
                        m_SpaceShip.transform.position,
                        m_SelectedTarget.transform.position,
                        targetRb.linearVelocity,
                        m_ProjectileSpeed);
                }

                // Поворачиваем корабль в сторону упреждения (или используем это как цель для турелей)
                m_MovePosition = aimPos;

                if (m_AIBehavior == AIBehavior.PatrolRoute)
                {
                    if (!IsTargetInPatrolRouteZone(m_SelectedTarget.transform.position))
                        return;
                }

                m_SpaceShip.Fire(TurrentMode.Primary);

                m_FireTimer.Start(m_ShootDelay);
            }
        }
    }

    private Destructible FindNearestDestructibleTarget()
    {
        float maxDist = float.MaxValue;

        Destructible potentialTarget = null;

        foreach (var v in Destructible.AllDestructibles)
        {
            if (v == m_SpaceShip) continue;
            if (v.TeamId == m_TeamId) continue;
            if (v.GetComponent<SpaceShip>() == m_SpaceShip) continue;
            if (v.TeamId == Destructible.TeamIdNeural) continue;
            if (v.TeamId == m_SpaceShip.TeamId) continue;

            if (m_AIBehavior == AIBehavior.PatrolRoute)
            {
                if (!IsTargetInPatrolRouteZone(v.transform.position))
                    continue;
            }

            float dist = Vector2.Distance(m_SpaceShip.transform.position, v.transform.position);

            if (dist < maxDist)
            {
                maxDist = dist;
                potentialTarget = v;
            }
        }

        return potentialTarget;
    }

    #region Timers

    private void InitTimers()
    {
        m_RandomizeDirectionTimer = new Timer(m_RandomSelectMovePointTime);

        m_FireTimer = new Timer(m_ShootDelay);

        m_FindNewTargetTimer = new Timer(m_FindNewTargetTime);
    }

    private void UpdateTimers()
    {
        m_RandomizeDirectionTimer.RemoveTime(Time.deltaTime);
        m_FireTimer.RemoveTime(Time.deltaTime);
        m_FindNewTargetTimer.RemoveTime(Time.deltaTime);
    }

    public void SetPatrolBehaviour(AIPointPatrol point)
    {
        m_AIBehavior = AIBehavior.Patrol;
        m_PatrolPoint = point;
        // m_MovePosition = point.transform.position;
    }

    public void SetPatrolRouteBehaviour(AIPointPatrol[] route)
    {
        m_AIBehavior = AIBehavior.PatrolRoute;
        m_PatrolRoute = route;
        m_CurrentPatrolIndex = 0;

        if (m_PatrolRoute != null && m_PatrolRoute.Length > 0)
        {
            m_MovePosition = m_PatrolRoute[0].transform.position;
        }
    }

    #endregion

    #region Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса достижения точки
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_PatrolPointRadius);

        // Визуализация маршрута патрулирования
        if (m_AIBehavior == AIBehavior.PatrolRoute && m_PatrolRoute != null && m_PatrolRoute.Length > 0)
        {
            Gizmos.color = Color.cyan;

            // Рисуем линии между точками маршрута
            for (int i = 0; i < m_PatrolRoute.Length; i++)
            {
                if (m_PatrolRoute[i] != null)
                {
                    // Радиус достижения для каждой точки
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(m_PatrolRoute[i].transform.position, m_PatrolPointRadius);

                    // Линии маршрута
                    Gizmos.color = Color.white;
                    if (i < m_PatrolRoute.Length - 1 && m_PatrolRoute[i + 1] != null)
                    {
                        Gizmos.DrawLine(m_PatrolRoute[i].transform.position, m_PatrolRoute[i + 1].transform.position);
                    }
                    else if (m_PatrolRoute[0] != null)
                    {
                        // Замыкаем маршрут от последней точки к первой
                        Gizmos.DrawLine(m_PatrolRoute[i].transform.position, m_PatrolRoute[0].transform.position);
                    }

                    // Номера точек

                    UnityEditor.Handles.Label(m_PatrolRoute[i].transform.position + Vector3.up * 0.5f, i.ToString());

                }
            }

            // Текущая целевая точка
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(m_MovePosition, 0.3f);
            }
        }

        // Визуализация лучей уклонения
        Gizmos.color = Color.magenta;
        float[] angles = { 0f, 45f, -45f, 90f, -90f };
        foreach (float angle in angles)
        {
            Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.up;
            Gizmos.DrawRay(transform.position, direction * m_EvadeRayLength);
        }

        // Визуализация упреждения
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_MovePosition, 0.3f);

            if (m_SelectedTarget != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_LeadPoint, 0.2f);
                Gizmos.DrawLine(m_SpaceShip.transform.position, m_LeadPoint);
            }
        }
    }
#endif
    #endregion
}

