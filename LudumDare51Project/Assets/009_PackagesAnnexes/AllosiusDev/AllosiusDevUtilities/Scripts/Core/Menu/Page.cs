//
// Updated by Allosius(Yanis Q.) on 10/9/2022.
//

using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

namespace AllosiusDevUtilities.Core
{
    
    namespace Menu {

        public class Page : MonoBehaviour
        {
            #region Fields

            //private Animator m_Animator;
            private DG.Tweening.Tween fadeTween;

            private bool m_IsOn;

            private PageController currentPageController;

            #endregion

            #region Properties

            public static readonly string FLAG_ON = "On";
            public static readonly string FLAG_OFF = "Off";
            public static readonly string FLAG_NONE = "None";

            public string targetState {get;private set;}
            public bool isOn {
                get {
                    return m_IsOn;
                }
                private set {
                    m_IsOn = value;
                }
            }

            public bool isActive { get; protected set; }

            #endregion

            #region UnityInspector

            public PageType type;
            public bool useAnimation;
            public float fadeAnimDuration;
            public bool canEscapeClose = true;

            [SerializeField] private CanvasGroup canvasGroup;

            #endregion

            #region Events

            public event Action onOpenPage;

            public event Action onClosePage;

            #endregion

            #region Unity Functions

            private void OnEnable() {
                CheckAnimatorIntegrity();
            }

            #endregion

            #region Public Functions
            /// <summary>
            /// Call this to turn the page on or off by setting the control '_on'
            /// </summary>
            public void Animate(bool _on, PageController pageController) 
            {
                currentPageController = pageController;

                if(_on)
                {
                    if(onOpenPage != null)
                        onOpenPage.Invoke();

                    isActive = true;
                }
                else
                {
                    if(onClosePage != null)
                        onClosePage.Invoke();

                    isActive = false;

                    if (currentPageController != null)
                    {
                        Log("CurrentPageController SetCanClosePage");
                        currentPageController.SetCanClosePage(true);
                    }
                }

                if (useAnimation) 
                {
                    //m_Animator.SetBool("on", _on);
                    if(_on)
                    {
                        Log("FadeIn");
                        FadeIn(fadeAnimDuration);
                    }
                    else
                    {
                        Log("FadeOut");
                        FadeOut(fadeAnimDuration);
                    }

                    Log("Launch Await Animation Coroutine");
                    StopCoroutine("AwaitAnimation");
                    StartCoroutine("AwaitAnimation", _on);
                } 
                else 
                {
                    if (!_on) 
                    {
                        isOn = false;
                        gameObject.SetActive(false);
                        canvasGroup.alpha = 0;
                    } 
                    else 
                    {
                        isOn = true;
                        canvasGroup.alpha = 1;
                    }

                    if(currentPageController != null)
                    {
                        currentPageController.SetCanClosePage(true);
                    }
                }
            }
#endregion

#region Private Functions

            private IEnumerator AwaitAnimation(bool _on) 
            {
                Log("Await Animation");

                targetState = _on ? FLAG_ON : FLAG_OFF;

                /*while (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState)) {
                    yield return null;
                }
                while (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                    yield return null;
                }*/

                if(useAnimation)
                {
                    Log("Wait Fade Anim Duration");
                    //yield return new WaitForSeconds(fadeAnimDuration);
                    yield return new WaitForSecondsRealtime(fadeAnimDuration);
                }

                targetState = FLAG_NONE;

                Log("Page ["+type+"] finished transitioning to "+(_on ? "<color=#0f0>on</color>." : "<color=#f00>off</color>."));

                if (!_on) 
                {
                    isOn = false;
                    gameObject.SetActive(false);
                } 
                else 
                {
                    isOn = true;

                    if (currentPageController != null)
                    {
                        Log("CurrentPageController SetCanClosePage");
                        currentPageController.SetCanClosePage(true);
                    }
                }

                
            }

            private void CheckAnimatorIntegrity() {
                if (useAnimation) {
                    // try to get animator
                    if(canvasGroup == null)
                    {
                        canvasGroup = GetComponent<CanvasGroup>();
                        if (!canvasGroup)
                        {
                            LogWarning("You opted to animate page [" + type + "], but no CanvasGroup component exists on the object.");
                        }
                    }
                    

                    /*m_Animator = GetComponent<Animator>();
                    if (!m_Animator) {
                        LogWarning("You opted to animate page ["+type+"], but no Animator component exists on the object.");
                    }*/
                }
            }


            public void FadeIn(float duration)
            {
                Debug.Log("FadeIn Launched");
                Fade(1f, duration, () =>
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                });
            }

            public void FadeOut(float duration)
            {
                Debug.Log("FadeOut Launched");

                Fade(0f, duration, () =>
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                });
            }

            private void Fade(float endValue, float duration, TweenCallback onEnd)
            {
                Debug.Log("Fade");

                if (fadeTween != null)
                {
                    fadeTween.Kill(false);
                }

                fadeTween = canvasGroup.DOFade(endValue, duration).SetUpdate(true);

                fadeTween.onComplete += onEnd;
            }


            private void Log(string _msg) {
                Debug.Log("[Page]: "+_msg);
            }

            private void LogWarning(string _msg) {
                Debug.LogWarning("[Page]: "+_msg);
            }
#endregion
        }
    }
}