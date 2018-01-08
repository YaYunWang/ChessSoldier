using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Profiling;

public delegate void OnBezierComplete();
public class SimpleTween : MonoBehaviour {
    public enum TweenType
    {
        Position,
        Scale,
        Alpha,
        Rotation,
    }

    public class TweenOperation
    {
        protected TweenType m_tweenType;
        protected AnimationCurve m_aniCurve;
        protected float m_velocity = 1f;
        protected bool m_reverse = false;
        protected float m_curve = 0f;
        protected float m_counterTween = 0;
        protected float m_totalTween = 0;
        protected bool m_isRun = false;
        protected Action<float> m_tweenFun;
        public TweenOperation(AnimationCurve aniCurve, float velocity , TweenType type, Action<float> tweenFun)
        {
            m_tweenType = type;
            m_aniCurve = aniCurve;
            m_velocity = velocity;
            m_tweenFun = tweenFun;
            m_totalTween = m_aniCurve.keys[m_aniCurve.length - 1].time;
        }

        public TweenType tweenType
        {
            get
            {
                return m_tweenType;
            }
        }

        private bool isFinish
        {
            get
            {
                if (m_counterTween > m_totalTween)
                {
                    if (m_reverse)
                        m_curve = 0;
                    else
                        m_curve = 1;
                    m_isRun = false;
                    return true;
                }
                return false;
          
            }
        }

        public void UpdateTween()
        {
            if (!m_isRun)
                return;

            if (!isFinish)
            {
                m_counterTween += Time.deltaTime / m_velocity;
                if (m_reverse)
                    m_curve = m_aniCurve.Evaluate(m_totalTween - m_counterTween);
                else
                    m_curve = m_aniCurve.Evaluate(m_counterTween);
            }
            
            m_tweenFun(m_curve);
            
        }

        public bool isDone
        {
            get { return !m_isRun; }
        }

        public void Play()
        {
            m_reverse = false;
            m_isRun = true;
            m_curve = 0;
            m_counterTween = 0;
        }

        public void ReversePlay()
        {
            Play();
            m_reverse = true;
        }

        public void Pause()
        {
            m_isRun = false;
        }

        public void Connitue()
        {
            if (!m_isRun)
                m_isRun = true;
        }

        public void Reset()
        {
            m_isRun = false;
            m_curve = 0;
            m_counterTween = 0;
            if (m_aniCurve != null)
                m_totalTween = m_aniCurve.keys[m_aniCurve.length - 1].time;

        }

        public void Reset(AnimationCurve aniCurve, float velocity)
        {
            m_aniCurve = aniCurve;
            m_velocity = velocity;
            Reset();
        }

    }
    
    public AnimationCurve positionCurve;
    [SerializeField]
    private List<Vector2> positions = new List<Vector2>();
    
    public AnimationCurve scaleCurve;
    [SerializeField]
    private List<Vector2> scales = new List<Vector2>();

    public AnimationCurve alphaCurve;
    [Range(0, 1)]
    public List<float> alphas = new List<float>();

    public AnimationCurve rotationCurve;
    [SerializeField]
    private List<Vector3> rotations = new List<Vector3>();

    [SerializeField]
    private bool Immediately = false;
    
    public float velocity = 1f;
    public bool reverse = false;
    public bool loop = false;

    public UnityEvent onFinish = null;
    public UnityEvent onLoopFinish = null;

    private CanvasGroup m_canvasGroup;
    private RectTransform m_rectTrans;

    private bool isRun = false;
    public bool IsRun
    {
        get { return isRun; }
    }

    private List<TweenOperation> m_operations = new List<TweenOperation>();

    //private Dictionary<TweenType, TweenOperation> m_tweens = new Dictionary<TweenType, TweenOperation>();
    
    
    void Awake()
    {
        m_rectTrans = GetComponent<RectTransform>();
        m_canvasGroup = GetComponent<CanvasGroup>();

        SetTweenPosition(positions);
        SetTweenScale(scales);
        SetTweenAlpha(alphas);
        SetTweenRotation(rotations);
    }

    // Use this for initialization
    void OnEnable()
    {
        if (Immediately)
        {
            if (reverse)
                ReversePlay();
            else
                Play();
        }
    }
	
    void LateUpdate()
    {
        if (!isRun)
            return;

        bool alldone = true;

        int count = m_operations.Count;
        Profiler.BeginSample("simT1 ");
        for (int i = 0; i < count; i++)
        {
            Profiler.BeginSample("simT2 ");
            var operation = m_operations[i];
            Profiler.EndSample();
            operation.UpdateTween();
            if (operation.isDone == false)
            {
                alldone = false;
            }
        }
        Profiler.EndSample();

        if (alldone)
        {
            isRun = false;
            if (loop)
            {
                if (onLoopFinish != null)
                    onLoopFinish.Invoke();
                if (reverse)
                    ReversePlay();
                else
                    Play();
            }
            else
            {
                if (onFinish != null)
                    onFinish.Invoke();
                reverse = false;
            }
        }
        
    }
    
    private void PositionTween(float curve)
    {
        int count = positions.Count;
        switch(count)
        {
            case 2:
                m_rectTrans.anchoredPosition = OnceBezier(curve, positions[0], positions[1]);
                break;
            case 3:
                m_rectTrans.anchoredPosition = SecondaryBezier(curve, positions[0], positions[1], positions[2]);
                break;
            default:

                break;
        }
    }

    private void ScaleTween(float curve)
    {
        int count = scales.Count;
        switch(count)
        {
            case 2:
                m_rectTrans.localScale = OnceBezier(curve, scales[0], scales[1]);
                break;
            case 3:
                m_rectTrans.localScale = SecondaryBezier(curve, scales[0], scales[1], scales[2]);
                break;
            default:

                break;
        }
    }

    private void AlphaTween(float curve)
    {
        if (m_canvasGroup == null)
            return;
        int count = alphas.Count;
        switch(count)
        {
            case 2:
                m_canvasGroup.alpha = FloatOnceBezier(curve, alphas[0], alphas[1]);
                break;
            case 3:
                m_canvasGroup.alpha = FloatSecondaryBezier(curve, alphas[0], alphas[1], alphas[2]);
                break;
            default:

                break;
        }
    }

    private void RotationTween(float curve)
    {
        int count = rotations.Count;
        switch (count)
        {
            case 2:
                m_rectTrans.localEulerAngles = OnceBezier(curve, rotations[0], rotations[1]);
                break;
            case 3:
                m_rectTrans.localEulerAngles = SecondaryBezier(curve, rotations[0], rotations[1], rotations[2]);
                break;
            default:

                break;
        }
    }

    public void SetTweenPosition(List<Vector2> positionslist, AnimationCurve anicurve = null)
    {
        if (anicurve != null)
            positionCurve = anicurve;

        if (positionslist == null || positionslist.Count == 0 || positionCurve == null || positionCurve.length == 0)
            return;

        positions = positionslist;
        ChangePoint1(positions);

        TweenOperation tween = m_operations.Find(p => p.tweenType == TweenType.Position);
        if (tween == null)
        {
            tween = new TweenOperation(positionCurve, velocity, TweenType.Position, PositionTween);
            m_operations.Add(tween);
        }
        else
        {
            tween.Reset(positionCurve, velocity);
        }
    }

    public void SetTweenScale(List<Vector2> scaleslist, AnimationCurve anicurve = null)
    {
        if (anicurve != null)
            scaleCurve = anicurve;

        if (scaleslist == null || scaleslist.Count == 0 || scaleCurve == null || scaleCurve.length == 0)
            return;

        scales = scaleslist;
        ChangePoint1(scales);

        TweenOperation tween = m_operations.Find(p => p.tweenType == TweenType.Scale);

        if (tween == null)
        {
            tween = new TweenOperation(scaleCurve, velocity, TweenType.Scale, ScaleTween);
            m_operations.Add(tween);
        }
        else
        {
            tween.Reset(scaleCurve, velocity);
        }
    }

    public void SetTweenAlpha(List<float> alphaslist, AnimationCurve anicurve = null)
    {
        if (anicurve != null)
            alphaCurve = anicurve;

        if (alphaslist == null || alphaslist.Count == 0 || alphaCurve == null || alphaCurve.length == 0)
            return;

        alphas = alphaslist;
        ChangeFloat1(alphas);

        TweenOperation tween = m_operations.Find(p => p.tweenType == TweenType.Alpha);

        if (tween == null)
        {
            tween = new TweenOperation(alphaCurve, velocity, TweenType.Alpha, AlphaTween);
            m_operations.Add(tween);
        }
        else
        {
            tween.Reset(alphaCurve, velocity);
        }
    }

    public void SetTweenRotation(List<Vector3> rotationlist, AnimationCurve anicurve = null)
    {
        if (anicurve != null)
            rotationCurve = anicurve;

        if (rotationlist == null || rotationlist.Count == 0 || rotationCurve == null || rotationCurve.length == 0)
            return;

        rotations = rotationlist;
        ChangePoint1(rotations);

        TweenOperation tween = m_operations.Find(p => p.tweenType == TweenType.Rotation);

        if (tween == null)
        {
            tween = new TweenOperation(rotationCurve, velocity, TweenType.Rotation, RotationTween);
            m_operations.Add(tween);
        }
        else
        {
            tween.Reset(rotationCurve, velocity);
        }
    }

    public void Play()
    {
        isRun = true;
        reverse = false;
        foreach (var operation in m_operations)
        {
            operation.Play();
        }
    }
    
    public void ReversePlay()
    {
        isRun = true;
        reverse = true;
        foreach (var operation in m_operations)
        {
            operation.ReversePlay();
        }
    }

    public void Pause()
    {
        isRun = false;
        foreach (var operation in m_operations)
        {
            operation.Pause();
        }
    }

    public void Connitue()
    {
        if (!isRun)
            isRun = true;
        foreach (var operation in m_operations)
        {
            operation.Connitue();
        }
    }

    public void Reset()
    {
        isRun = false;
        foreach (var operation in m_operations)
        {
            operation.Reset();
        }
    }

    private bool ExecuteTween(TweenType tweentype)
    {
        TweenOperation tween = m_operations.Find(p => p.tweenType == tweentype);
        
        if (tween != null)
        {
            tween.UpdateTween();
            return tween.isDone;
        }
        return true;
    }

    private Vector3 OnceBezier(float t, Vector3 p0, Vector3 p1)
    {
        return (1 - t) * p0 + t * p1;
    }
    
    private Vector3 SecondaryBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
    
    private float FloatOnceBezier(float t, float f0, float f1)
    {
        return (1 - t) * f0 + t * f1;
    }

    private float FloatSecondaryBezier(float t, float f0, float f1, float f2)
    {
        return (1 - t) * (1 - t) * f0 + 2 * (1 - t) * t * f1 + t * t * f2;
    }

    private Vector3 GetPointC(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3 pc = Vector3.zero;

        float disP0P1 = Vector3.Distance(p0, p1);
        float disP2P1 = Vector3.Distance(p2, p1);
        pc = p1 - 0.5f * Mathf.Sqrt(disP0P1*disP2P1)*((p0-p1)/disP0P1 + (p2-p1)/disP2P1);
        return pc;
    }

    private float GetFloatC(float f0, float f1, float f2)
    {
        float disF0F1 = Mathf.Abs(f0 - f1);
        float disF2F1 = Mathf.Abs(f2 - f1);
        float fc = f1 - 0.5f * Mathf.Sqrt(disF0F1 * disF2F1) * ((f0 - f1) / disF0F1 + (f2 - f1) / disF2F1);
        return fc;
    }

    private void ChangePoint1(List<Vector2> list)
    {
        if (list.Count == 3)
            list[1] = GetPointC(list[0], list[1], list[2]);
    }

    private void ChangePoint1(List<Vector3> list)
    {
        if (list.Count == 3)
            list[1] = GetPointC(list[0], list[1], list[2]);
    }

    private void ChangeFloat1(List<float> list)
    {
        if (list.Count == 3)
            list[1] = GetFloatC(list[0], list[1], list[2]);
    }
    
}
