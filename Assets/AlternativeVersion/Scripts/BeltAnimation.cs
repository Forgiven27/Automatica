using DG.Tweening;
using UnityEngine;

public class BeltAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 _positionStart;
    [SerializeField] private Vector3 _positionEnd;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private Transform _container;
    [SerializeField] private int _objectCount;
    [SerializeField] private float _time;
    [SerializeField] private float _delay;
    [SerializeField] private Ease _ease;
    private GameObject[] _pool;
    float timer = 0;
    int index = 0;
    Sequence[] _sequences;
    void Start()
    {
        _pool = new GameObject[_objectCount];
        _sequences = new Sequence[_objectCount];

        
        for (int i = 0; i < _pool.Length; i++)
        {
            var go = Instantiate(_gameObject, _positionStart, Quaternion.LookRotation(_positionEnd - _positionStart, Vector3.up), _container);

            _pool[i] = go;
            go.SetActive(false);
            _sequences[i] = DOTween.Sequence()
            .PrependCallback(() =>
            {
                go.SetActive(true);
                go.transform.position = _positionStart;
                Debug.Log(go.transform.position);
            })
            .Append(go.transform.DOMove(_positionEnd, _time).SetEase(_ease))
            .AppendCallback(() => go.SetActive(false))
            .SetAutoKill(false)
            .Pause();
        }


    }
    
    void Update()
    {
        if (_pool == null || _pool.Length == 0) return;
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }
        if (index >= _pool.Length) index = 0;
        if (!_sequences[index].IsPlaying())
        {
            _sequences[index].Restart();
        }
        timer = _delay;
        index++;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_positionStart, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_positionEnd, 0.5f);
    }

    private void OnDestroy()
    {
        if (_sequences != null)
        {
            foreach (var seq in _sequences)
            {
                seq?.Kill();
            }
        }
    }
}
