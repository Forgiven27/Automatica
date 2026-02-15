using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Follow : MonoBehaviour
{
    public enum ForwardAxis { Z, X }

    [Header("Targets & Pivot")]
    public Transform pivot;           // точка вращения (если null — используются transform)
    public Transform followTarget;    // цель, на которую смотрим

    [Header("Plane / Axis")]
    public Vector3 rotationAxis = Vector3.up; // ось вращения (локальная или мировая, см. useLocalAxis)
    public bool useLocalAxis = true;          // если true — axis в локальных координатах pivot

    [Header("Forward")]
    public ForwardAxis forward = ForwardAxis.Z; // ось модели, считающаяся "вперёд"

    [Header("Limits")]
    public float minAngle = -90f; // в градусах
    public float maxAngle = 90f;

    [Header("Options")]
    public bool snapIfBehind = false; // если проекция совпадает с pivot (или очень близко) — не вращать

    // начальная ориентация (локальная) в качестве нулевой позиции ограничений
    private Quaternion baseLocalRotation;
    private Vector3 baseDir; // базовое направление, относительно которого считаем угол

    void Start()
    {
        if (pivot == null) pivot = transform;
        // Запомним базовую ориентацию и базовое направление (в мировых координатах)
        baseLocalRotation = transform.localRotation;

        Vector3 axisWorld = GetAxisWorld();
        baseDir = GetForwardWorld(baseLocalRotation);
        // Проецируем baseDir на плоскость, чтобы получить корректную "нулевую" проекцию
        baseDir = ProjectOnPlaneDirectional(baseDir.normalized, axisWorld).normalized;
        if (baseDir.sqrMagnitude < 1e-6f)
            baseDir = Vector3.forward; // запасной вариант
    }

    void Update()
    {
        if (followTarget == null || pivot == null) return;

        Vector3 P = pivot.position;
        Vector3 T = followTarget.position;

        // Нормаль плоскости = axis (в мировой системе)
        Vector3 axisWorld = GetAxisWorld().normalized;

        // Проекция точки T на плоскость, проходящую через P
        Vector3 Tproj = T - Vector3.Dot(T - P, axisWorld) * axisWorld;

        // Если проекция совпала с pivot (цель прямо по оси), можно не крутить
        Vector3 toProj = Tproj - P;
        if (toProj.sqrMagnitude < 1e-6f)
        {
            if (snapIfBehind) return;
            // либо не меняем, либо оставляем минимальное поворотное значение
        }

        Vector3 dir = toProj.normalized;

        // Мы хотим сравнивать в плоскости вращения — проецируем базовый forward в ту же плоскость
        Vector3 baseDirWorld = baseDir;
        baseDirWorld = ProjectOnPlaneDirectional(baseDirWorld, axisWorld).normalized;

        // Теперь вычисляем signed angle в градусах (положит = по правому правилу вокруг axisWorld)
        float signed = Vector3.SignedAngle(baseDirWorld, dir, axisWorld);

        // Ограничиваем угол
        float clamped = Mathf.Clamp(signed, minAngle, maxAngle);

        // Применяем поворот = базовая вращение + вращение вокруг axisWorld на clamped
        // Если база задана в локальной системе, нужно преобразовать axis в локальную систему для корректного умножения.
        // Но проще: строим мировую ротацию:
        Quaternion worldBaseRot = pivot.rotation * baseLocalRotation; // приблизительно базовая мировая ориентация
        Quaternion rotDelta = Quaternion.AngleAxis(clamped, axisWorld);
        Quaternion finalWorldRot = rotDelta * worldBaseRot;

        // Теперь если мы хотим сохранить локальную ориентацию относ. родителя:
        if (transform.parent != null)
            transform.rotation = finalWorldRot;
        else
            transform.rotation = finalWorldRot;

        // Если forward = X, можно компенсировать (если нужно чтобы 'вперёд' - X действительно смотрел в dir)
        // В этом варианте мы уже вращаем вокруг axisWorld, а forward-осевую компенсацию делаем в базовой локальной ротации.
    }

    // helper: получить axis в мировых координатах
    Vector3 GetAxisWorld()
    {
        if (pivot == null) return rotationAxis.normalized;
        return useLocalAxis ? pivot.TransformDirection(rotationAxis.normalized) : rotationAxis.normalized;
    }

    // helper: проекция направления v на плоскость с нормалью n (ориентированная на направлении)
    static Vector3 ProjectOnPlaneDirectional(Vector3 v, Vector3 n)
    {
        return v - Vector3.Dot(v, n) * n;
    }

    // helper: получить "forward" в мировых координатах из локальной ориентации
    Vector3 GetForwardWorld(Quaternion localRot)
    {
        // forward зависит от того, какой axis считается "вперёд" у модели
        if (pivot == null) return (forward == ForwardAxis.Z) ? transform.forward : transform.right;
        if (forward == ForwardAxis.Z)
            return pivot.rotation * (localRot * Vector3.forward);
        else
            return pivot.rotation * (localRot * Vector3.right);
    }




    /*
    [SerializeField] GameObject followObject;

    [SerializeField] private bool isXBlock;
    [SerializeField] private bool isYBlock;
    [SerializeField] private bool isZBlock;

    Vector3 startAngles;
    void Start()
    {
        startAngles = transform.eulerAngles;
    }

    
    void Update()
    {
        
        Vector3 followConstraintPos = new Vector3(
            isXBlock ? transform.position.x : followObject.transform.position.x,
            isYBlock ? transform.position.y : followObject.transform.position.y,
            isZBlock ? transform.position.z : followObject.transform.position.z
            );
        Vector3 dir = followConstraintPos
            - transform.position;

        float angle = Vector3.Angle(dir, transform.forward);
        
        
        
        
        print(dir.normalized);
        Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
        target = target * Quaternion.Euler(0, -90, 0);
        
        
        var forwardThis = Quaternion.LookRotation(transform.forward, Vector3.up);
        

        Vector3 e = target.eulerAngles;
        
        transform.rotation = Quaternion.Euler(e);
    }

    */

}
