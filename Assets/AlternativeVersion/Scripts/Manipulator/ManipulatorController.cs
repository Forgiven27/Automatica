using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaCoroutine = MoonSharp.Interpreter.Coroutine;
using DG.Tweening;

namespace DullVersion
{
    public class ManipulatorController : MonoBehaviour
    {
        [SerializeField] private string _name;

        [SerializeField] private JointController[] _joints;
        [SerializeField] private HandIntersects _handIntersects;
        [SerializeField] private Transform _handGrabPlacement;

        float _time = 0f;
        bool _isScannig = false;
        ItemFilter _filter = new();
        bool _isGrabbed = false;
        ObjectInfo _grabbedObject;

        List<Operation> operations = new List<Operation>();
        int _opIndex = 0;
        



        private bool _IsStopScript = false;
        IEnumerator DoCor(LuaCoroutine cor)
        {
            while (cor.State != CoroutineState.Dead && !_IsStopScript)
            {
                DynValue sec = cor.Resume();
                yield return new WaitForSeconds((float)sec.Number);
            }
        }

        public void SetAndStartScript(string userScript)
        {

            string luaScript = $@"
            function Wait(seconds)
              coroutine.yield(seconds)
            end

            function Main()
              {userScript}
            end
            function NotMain()
              Write('Выполнилось что-то не то')
            end";
            Script script = new Script();

            script.Options.DebugPrint = s => Debug.Log(s);


            script.Globals["IsGrab"] = (Func<bool>)IsGrabItem;
            
            #if UNITY_EDITOR
            script.Globals["Write"] = (Action<string>)Debug.Log;
            #endif
            
            script.Globals["RotateJoint"] = (Action<int, float>)RotateJoint;
            script.Globals["GrabNow"] = (Action)Grab;
            script.Globals["ReleaseNow"] = (Action)Release;
            
            
            script.Globals["SetItemsWhiteList"] = (Action<string[]>)SetFilter_Item_WhiteList_FromString;
            script.Globals["SetItemsBlackList"] = (Action<string[]>)SetFilter_Item_BlackList_FromString;
            script.Globals["SetQualityWhiteList"] = (Action<string[]>)SetFilter_Quality_WhiteList_FromString;
            script.Globals["SetQualityBlackList"] = (Action<string[]>)SetFilter_Quality_BlackList_FromString;

            script.Globals["ClearItemsWhiteList"] = (Action)Clear_Items_WhiteList;
            script.Globals["ClearItemsBlackList"] = (Action)Clear_Items_BlackList;
            script.Globals["ClearQualityWhiteList"] = (Action)Clear_Quality_WhiteList;
            script.Globals["ClearQualityBlackList"] = (Action)Clear_Quality_BlackList;




            script.DoString(luaScript);

            DynValue main = script.Globals.Get("Main");

            LuaCoroutine cor = script.CreateCoroutine(script.Globals.Get("Main")).Coroutine;
            StartCoroutine("DoCor", cor);
        }



        bool IsGrabItem()
        {
            return _isGrabbed;
        }



        

        public void RotateJoint(int index, float angle)
        {
            if (index >= _joints.Length || index < 0) return;
            _joints[index].RotateTo(angle);
        }
        public void AddOperation(Operation operation)
        {
            operations.Add(operation);
        }

        public void ClearOperations()
        {
            operations.Clear();
        }
        public void AddWaitTime(float value)
        {
            _time += value;
        }

        public void Grab()
        {
            if (_isGrabbed) return;
            if (_handIntersects.TryGetItem(out ObjectInfo objectInfo))
            {
                _grabbedObject = objectInfo;
                _grabbedObject.TryGetComponent(out Rigidbody rb);
                if (_filter.isEmptyFilter)
                {
                    _grabbedObject.transform.SetParent(_handGrabPlacement);
                    _grabbedObject.transform.DOLocalMove(Vector3.zero, 1);
                    _isGrabbed = true;
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }
                }
                else
                {
                    if (_filter.CheckObject(objectInfo))
                    {
                        _grabbedObject.transform.SetParent(_handGrabPlacement);
                        _grabbedObject.transform.DOLocalMove(Vector3.zero, 1);
                        _isGrabbed = true;
                        if (rb != null)
                        {
                            rb.isKinematic = true;
                        }
                    }
                }
            }
        }

        public void Release()
        {
            if (!_isGrabbed) return;
            if (_grabbedObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            _grabbedObject.transform.SetParent(null);
            _grabbedObject = null;
            _isGrabbed = false;
        }

        public void EnableScaner()
        {
            _isScannig = true;
        }

        public void DisableScaner()
        {
            _isScannig = false;
        }

        public void SetFilter(ItemFilter filter)
        {
            _filter = filter;
        }


        public void SetFilter_Item_WhiteList_FromString(string[] items)
        {
            if (_filter.itemsWhiteList == null) _filter.itemsWhiteList = new();
            if (_filter.itemsBlackList == null) _filter.itemsBlackList = new();
            foreach (string item in items)
            {
                if (string.IsNullOrEmpty(item)) continue;

                switch (item.ToLowerInvariant())
                {
                    case "iron":
                        if (!_filter.itemsWhiteList.Contains(Item.Iron))
                            _filter.itemsWhiteList.Add(Item.Iron);
                        if (_filter.itemsBlackList.Contains(Item.Iron))
                            _filter.itemsBlackList.Remove(Item.Iron);
                        break;
                    case "copper":
                        if (!_filter.itemsWhiteList.Contains(Item.Copper))
                            _filter.itemsWhiteList.Add(Item.Copper);
                        if (_filter.itemsBlackList.Contains(Item.Copper))
                            _filter.itemsBlackList.Remove(Item.Copper);
                        break;
                }
            }
        }

        public void SetFilter_Item_BlackList_FromString(string[] items)
        {
            if (_filter.itemsWhiteList == null) _filter.itemsWhiteList = new();
            if (_filter.itemsBlackList == null) _filter.itemsBlackList = new();
            foreach (string item in items)
            {
                if (string.IsNullOrEmpty(item)) continue;

                switch (item.ToLowerInvariant())
                {
                    case "iron":
                        if (!_filter.itemsBlackList.Contains(Item.Iron))
                            _filter.itemsBlackList.Add(Item.Iron);
                        if (_filter.itemsWhiteList.Contains(Item.Iron))
                            _filter.itemsWhiteList.Remove(Item.Iron);
                        break;
                    case "copper":
                        if (!_filter.itemsBlackList.Contains(Item.Copper))
                            _filter.itemsBlackList.Add(Item.Copper);
                        if (_filter.itemsWhiteList.Contains(Item.Copper))
                            _filter.itemsWhiteList.Remove(Item.Copper);
                        break;
                }
            }
        }
        public void SetFilter_Quality_WhiteList_FromString(string[] qualities)
        {
            if (_filter.qualityWhiteList == null) _filter.qualityWhiteList = new();
            if (_filter.qualityBlackList == null) _filter.qualityBlackList = new();
            foreach (string quality in qualities)
            {
                if (string.IsNullOrEmpty(quality)) continue;

                switch (quality.ToLowerInvariant())
                {
                    case "low":
                        if (!_filter.qualityWhiteList.Contains(ItemQuality.LowQuality))
                            _filter.qualityWhiteList.Add(ItemQuality.LowQuality);
                        if (_filter.qualityBlackList.Contains(ItemQuality.LowQuality))
                            _filter.qualityBlackList.Remove(ItemQuality.LowQuality);
                        break;
                    case "medium":
                        if (!_filter.qualityWhiteList.Contains(ItemQuality.MediumQuality))
                            _filter.qualityWhiteList.Add(ItemQuality.MediumQuality);
                        if (_filter.qualityBlackList.Contains(ItemQuality.MediumQuality))
                            _filter.qualityBlackList.Remove(ItemQuality.MediumQuality);
                        break;
                    case "high":
                        if (!_filter.qualityWhiteList.Contains(ItemQuality.HighQuality))
                            _filter.qualityWhiteList.Add(ItemQuality.HighQuality);
                        if (_filter.qualityBlackList.Contains(ItemQuality.HighQuality))
                            _filter.qualityBlackList.Remove(ItemQuality.HighQuality);
                        break;
                }
            }
        }

        public void SetFilter_Quality_BlackList_FromString(string[] qualities)
        {
            if (_filter.qualityWhiteList == null) _filter.qualityWhiteList = new();
            if (_filter.qualityBlackList == null) _filter.qualityBlackList = new();
            foreach (string quality in qualities)
            {
                if (string.IsNullOrEmpty(quality)) continue;

                switch (quality.ToLowerInvariant())
                {
                    case "low":
                        if (!_filter.qualityBlackList.Contains(ItemQuality.LowQuality))
                            _filter.qualityBlackList.Add(ItemQuality.LowQuality);
                        if (_filter.qualityWhiteList.Contains(ItemQuality.LowQuality))
                            _filter.qualityWhiteList.Remove(ItemQuality.LowQuality);
                        break;
                    case "medium":
                        if (!_filter.qualityBlackList.Contains(ItemQuality.MediumQuality))
                            _filter.qualityBlackList.Add(ItemQuality.MediumQuality);
                        if (_filter.qualityWhiteList.Contains(ItemQuality.MediumQuality))
                            _filter.qualityWhiteList.Remove(ItemQuality.MediumQuality);
                        break;
                    case "high":
                        if (!_filter.qualityBlackList.Contains(ItemQuality.HighQuality))
                            _filter.qualityBlackList.Add(ItemQuality.HighQuality);
                        if (_filter.qualityWhiteList.Contains(ItemQuality.HighQuality))
                            _filter.qualityWhiteList.Remove(ItemQuality.HighQuality);
                        break;
                }
            }
        }

        public void Clear_Items_WhiteList()
        {
            _filter.itemsWhiteList?.Clear();
        }

        public void Clear_Items_BlackList()
        {
            _filter.itemsBlackList?.Clear();
        }

        public void Clear_Quality_WhiteList()
        {
            _filter.qualityWhiteList?.Clear();
        }

        public void Clear_Quality_BlackList()
        {
            _filter.qualityBlackList?.Clear();
        }

        /*
        public void SetRunner(LuaRunner runner)
        {
            _runner = runner;
        }

        private void Update()
        {
            if (_runner == null) return;
            _runner.Tick(Time.deltaTime);
        }

        */

        /*
        private void Update()
        {
            if (_time > 0f)
            {
                _time -= Time.deltaTime;
                return;
            }
            if (_isScannig)
            {
                Grab();
                if (_isGrabbed) _isScannig = false;
                else return;
            }
            if (operations.Count > 0 && _opIndex < operations.Count )
            {
                operations[_opIndex].Execute(this);
                if (_opIndex == operations.Count -1 ) _opIndex = 0;
                else _opIndex++;
            }

        }
        */

    }
}