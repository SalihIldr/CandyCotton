using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Winding : MonoBehaviour
{
    [SerializeField] private TemplateBasicCotton _templateBasicCotton;
    [SerializeField] private GameObject _templateCottonFluff;
    [SerializeField] private GameObject _templateCotttonEffects;
    [SerializeField] private Transform _spawner;

    private List<TemplateBasicCotton> _basicCottons;
    private GameObject _cottonEffects;
    private float _scale = 1f;
    private float _scaleZ = 1f;
    private float _offsetNextBasicCotton = 0.01f;
    private float _coefficientReductionNextBaseWool = 5;
    private Vector3 _offsetFluffCotton = new Vector3(0.2f, 0.2f, 0.1f);
    private Vector3 _offsetEffectCotton = new Vector3(2f, 2f, 0f);

    public event UnityAction DeformationStoped;
    public event UnityAction DeformationStarted;

    private void Start()
    {
        _basicCottons = new List<TemplateBasicCotton>();
        TemplateBasicCotton BasicCotton = Instantiate(_templateBasicCotton, _spawner);
        _basicCottons.Add(BasicCotton);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<CottonBlock>(out CottonBlock cottonBlock))
        {
            DeformationStarted?.Invoke();
            StartCoroutine(WrapingBasicCotton(cottonBlock.Rend, cottonBlock.CottonCreated));
            WrappingAdditionalCotton(cottonBlock.Rend, cottonBlock.CottonCreated);
            cottonBlock.CottonCreated = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.TryGetComponent<CottonBlock>(out CottonBlock cottonBlock))
        {
            DeformationStoped?.Invoke();
            StopCoroutine(WrapingBasicCotton(cottonBlock.Rend, cottonBlock.CottonCreated));
            Destroy(_cottonEffects);
        }
    }

    private void WrappingAdditionalCotton(Renderer Render, bool CottonCreated)
    {
        if (CottonCreated == false)
        {
            GameObject CottonFluff = Instantiate(_templateCottonFluff, _spawner);
            CottonFluff.transform.localScale = new Vector3(_scale, _scale, _scaleZ) + _offsetFluffCotton;
            _cottonEffects = Instantiate(_templateCotttonEffects, _spawner);
            Renderer materialCottonEffects = _cottonEffects.GetComponent<Renderer>();
            _cottonEffects.transform.localScale = new Vector3(_scale, _scale, _scaleZ) + _offsetEffectCotton;
            materialCottonEffects.material.color = Render.material.color;
        }
    }

    private IEnumerator WrapingBasicCotton(Renderer Render, bool CottonCreated)
    {
        WaitForSeconds wait = new WaitForSeconds(0.8f);

        if (CottonCreated == false)
        {
            TemplateBasicCotton Cotton = Instantiate(_templateBasicCotton, _spawner);
            _basicCottons.Add(Cotton);
            Renderer materialCotton = Cotton.GetComponent<Renderer>();
            Cotton.transform.localPosition = new Vector3(0, 0, 0);
            if (_basicCottons.Count >= 1)
            {
                _basicCottons[_basicCottons.Count - 2].WindingStop();
                _scale = _basicCottons[_basicCottons.Count - 2].Scale + _offsetNextBasicCotton;
                _scaleZ = _basicCottons[_basicCottons.Count - 2].ScaleZ / _coefficientReductionNextBaseWool;
                _basicCottons[_basicCottons.Count - 1].ScaleZControlInit(_basicCottons[_basicCottons.Count - 2].ScaleZ);
            }
            Cotton.transform.localScale = new Vector3(_scale, _scale, _scaleZ);
            materialCotton.material.color = Render.material.color;
        }
        yield return wait;
    }
}
