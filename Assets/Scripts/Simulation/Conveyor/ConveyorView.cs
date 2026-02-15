using System.Text;
using UnityEngine;
using Simulator;
using System.Collections.Generic;
using UnityEngine.Splines;
using System.Linq;

public class ConveyorView : MonoBehaviour, IEntity
{
    [SerializeField] private InfoUI infoUI;
    [SerializeField] private ConveyorModule _conveyorModule;
    [SerializeField] private ItemsInfo _itemsInfo;
    private List<ConveyorItem> _conveyorItems;
    public string ID { get; set; }
    public GameObject factoryMesh;
    public Texture2D icon;
    StringBuilder stringBuilder = new();

    private List<Vector3> _controlConveyorDots = new();
    public void Bind(string id)
    {
        ID = id;
    }

    private void OnEnable()
    {
        _conveyorModule.OnConveyorChanged += UpdateDots;
    }

    private void OnDisable ()
    {
        _conveyorModule.OnConveyorChanged -= UpdateDots;
    }

    void LateUpdate()
    {
        if (LayerMask.LayerToName(gameObject.layer) == "Building")
        {
            var snapshot = SimulationAPI.GetConveyor(ID);
            _conveyorItems = snapshot.items;
            stringBuilder.Clear();
            foreach (var item in snapshot.items)
            {
                stringBuilder.AppendLine($"Count {item.itemType} = {item.countItems} \nlinePlaceID = {item.linePlaceID}");
            }
            //infoUI.UpdateTextUI(stringBuilder.ToString());
            UpdateAllPositions();
        }
        //TODO Имитация белтов только коллизия, остальное генерация
    }

    void UpdateDots()
    {
        var conveyors = _conveyorModule.GetPoolConveyor();
        if (conveyors != null)
        {
            _controlConveyorDots.Clear();
            var next = conveyors.Find(x => x.IsFirstElement);
            if (next == null) return;
            _controlConveyorDots.Add(next.transform.position + Vector3.up);
            for (int i = 0; i < conveyors.Count - 1; i++)
            {
                if(next.NextConveyor != null)
                {
                    _controlConveyorDots.Add(next.NextConveyor.transform.position + Vector3.up); 
                    next = next.NextConveyor;
                }
            }

        }
        UpdateAllPositions();
    }



    
    [SerializeField] private float conveyorLength = 50f;
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private bool useMaterialPropertyBlock = true;

    // Храним данные отдельно по типам
    private Dictionary<ItemData, List<Matrix4x4>> matricesByType =
        new Dictionary<ItemData, List<Matrix4x4>>();

    private Dictionary<ItemData, List<Vector3>> positionsByType =
        new Dictionary<ItemData, List<Vector3>>();

    // MaterialPropertyBlock для вариативности внутри типа
    private Dictionary<ItemData, MaterialPropertyBlock> propertyBlocks =
        new Dictionary<ItemData, MaterialPropertyBlock>();

    void Start()
    {
        InitializeAllTypes();
        UpdateAllPositions();
    }

    void InitializeAllTypes()
    {
        if (_conveyorItems == null) return;
        if (_controlConveyorDots.Count < _conveyorItems.Count) 
        {
            UpdateDots(); 
            return; 
        }

        List<ItemData> list = new List<ItemData>();
        
        foreach (var conveyorItem in _conveyorItems)
        {
            var itemData = _itemsInfo.itemsData.Find(x => x.type == conveyorItem.itemType);
            if (!list.Contains(itemData))
            {
                list.Add(itemData);
            }
        }

        foreach (var itemData in list)
        {
            // Пропускаем если нет меша или материала
            if (itemData.mesh == null || itemData.material == null)
            {
                Debug.LogWarning($"Item type {itemData.name} missing mesh or material");
                continue;
            }

            // Инициализируем списки
            var matrices = new List<Matrix4x4>();
            var positions = new List<Vector3>();

            // Создаем начальные позиции (можно рандомно распределить)
            List<ConveyorItem> typedConveyorItemList = _conveyorItems.Where(x => x.itemType == itemData.type).ToList();
            for (int i = 0; i < typedConveyorItemList.Count(); i++)
            {
                // Распределяем равномерно по длине конвейера
                //float t = Random.Range(0f, 1f);
                
                Vector3 pos = _controlConveyorDots[typedConveyorItemList[i].linePlaceID] + Vector3.up / 2;

                positions.Add(pos);
                matrices.Add(CreateMatrix(pos, Vector3.one));
            }

            // Сохраняем
            matricesByType[itemData] = matrices;
            positionsByType[itemData] = positions;

            // Создаем MaterialPropertyBlock для этого типа
            if (useMaterialPropertyBlock)
            {
                var block = new MaterialPropertyBlock();

                // Пример: задаем случайные цвета для каждого инстанса
                Vector4[] colors = new Vector4[typedConveyorItemList.Count];
                for (int i = 0; i < typedConveyorItemList.Count; i++)
                {
                    colors[i] = new Color(
                        Random.Range(0.7f, 1f),
                        Random.Range(0.7f, 1f),
                        Random.Range(0.7f, 1f),
                        1f
                    );
                }
                block.SetVectorArray("_Color", colors);

                propertyBlocks[itemData] = block;
            }
        }
    }

    Matrix4x4 CreateMatrix(Vector3 position, Vector3 scale)
    {
        // Можно добавить небольшую рандомизацию вращения
        Quaternion rotation = Quaternion.Euler(
            0,
            Random.Range(0f, 360f),
            0
        );

        return Matrix4x4.TRS(position, rotation, scale);
    }

    void Update()
    {

        if (matricesByType.Count == 0 || positionsByType.Count == 0 || propertyBlocks.Count == 0)
        {
            InitializeAllTypes();
        }


        // Обновляем позиции для всех типов


        // Отрисовываем все типы


        RenderAllTypes();
    }

    void UpdateAllPositions()
    {
        foreach (var kvp in matricesByType)
        {
            var itemData = kvp.Key;
            var matrices = kvp.Value;
            var positions = positionsByType[itemData];
            positions.Clear();
            matrices.Clear();
            List<ConveyorItem> typedConveyorItemList = _conveyorItems.
                Where(x => x.itemType == itemData.type).ToList();

            // Обновляем каждую позицию
            for (int i = 0; i < typedConveyorItemList.Count; i++)
            {
                Vector3 pos = _controlConveyorDots[typedConveyorItemList[i].linePlaceID] + Vector3.up / 2;
                positions.Add(pos);

                // Обновляем матрицу
                matrices.Add(CreateMatrix(pos, Vector3.one));
            }
        }
    }

    void RenderAllTypes()
    {
        foreach (var kvp in matricesByType)
        {
            var itemData = kvp.Key;
            var matrices = kvp.Value;

            // Батчим по 1023 объекта за вызов (ограничение Unity)
            int batchCount = Mathf.CeilToInt((float)matrices.Count / 1023);

            for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
            {
                int startIndex = batchIndex * 1023;
                int count = Mathf.Min(1023, matrices.Count - startIndex);

                // Создаем подмассив для этого батча
                var batchMatrices = new Matrix4x4[count];
                matrices.CopyTo(startIndex, batchMatrices, 0, count);

                // Выбираем MaterialPropertyBlock для этого типа
                MaterialPropertyBlock block = null;
                if (useMaterialPropertyBlock && propertyBlocks.ContainsKey(itemData))
                {
                    block = propertyBlocks[itemData];

                    // Если используем цвет для каждого инстанса, нужно обновить подмассив
                    // Для оптимизации можно обновлять только если изменился
                }

                // Рисуем батч
                Graphics.DrawMeshInstanced(
                    itemData.mesh,
                    0, // submesh index
                    itemData.material,
                    batchMatrices,
                    count,
                    block,
                    UnityEngine.Rendering.ShadowCastingMode.On,
                    true // receive shadows
                );
            }
        }
    }

    // Метод для динамического добавления предметов
    public void AddItem(ItemData type, Vector3 position)
    {
        if (!matricesByType.ContainsKey(type))
        {
            // Если это новый тип, инициализируем для него списки
            matricesByType[type] = new List<Matrix4x4>();
            positionsByType[type] = new List<Vector3>();
        }

        positionsByType[type].Add(position);
        matricesByType[type].Add(CreateMatrix(position, Vector3.one));

        // Нужно обновить MaterialPropertyBlock (добавить цвет для нового элемента)
        if (useMaterialPropertyBlock && propertyBlocks.ContainsKey(type))
        {
            // Обновляем блок - пересоздаем или добавляем
            UpdatePropertyBlockForType(type);
        }
    }

    void UpdatePropertyBlockForType(ItemData type)
    {
        // Пересоздаем MaterialPropertyBlock с учетом нового количества
        var block = new MaterialPropertyBlock();
        int count = positionsByType[type].Count;

        Vector4[] colors = new Vector4[count];
        for (int i = 0; i < count; i++)
        {
            colors[i] = new Color(
                Random.Range(0.7f, 1f),
                Random.Range(0.7f, 1f),
                Random.Range(0.7f, 1f),
                1f
            );
        }
        block.SetVectorArray("_Color", colors);

        propertyBlocks[type] = block;
    }

    // Отладка: показываем статистику
    void OnGUI()
    {
        GUILayout.Label($"=== Conveyor Instancing Stats ===");
        int totalObjects = 0;
        int totalBatches = 0;

        foreach (var kvp in matricesByType)
        {
            int count = kvp.Value.Count;
            int batches = Mathf.CeilToInt((float)count / 1023);

            GUILayout.Label($"{kvp.Key.name}: {count} objects, {batches} batches");

            totalObjects += count;
            totalBatches += batches;
        }

        GUILayout.Label($"Total: {totalObjects} objects, {totalBatches} batches");
        GUILayout.Label($"Without instancing: {totalObjects} draw calls");
        GUILayout.Label($"With instancing: {totalBatches} draw calls");
    }

















}
