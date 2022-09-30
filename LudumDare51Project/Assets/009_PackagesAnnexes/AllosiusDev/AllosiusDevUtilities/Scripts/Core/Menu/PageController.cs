//
// Updated by Allosius(Yanis Q.) on 7/9/2022.
//

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace AllosiusDevUtilities.Core
{
    
    namespace Menu {

        public class PageController : MonoBehaviour
        {
            public bool debug;
            public PageType entryPage;
            public Page[] pages;

            private bool canClosePage = true;

            private Hashtable m_Pages;
            //private List<Page> m_OnList;
            //private List<Page> m_OffList;

#region Unity Functions
            private void Awake() 
            {
                m_Pages = new Hashtable();
                //m_OnList = new List<Page>();
                //m_OffList = new List<Page>();
                RegisterAllPages();

                if (entryPage != PageType.None)
                {
                    TurnTypePagesOn(entryPage);
                }
            }

            private void Update()
            {
                if (Input.GetButtonDown("Escape") && canClosePage)
                {
                    for (int i = pages.Length-1; i >= 0; i--)
                    {
                        Log(pages[i].gameObject.name);

                        if(pages[i].isActive && pages[i].canEscapeClose)
                        {
                            TurnPageOff(pages[i]);
                            break;
                        }
                    }
                }
            }

            #endregion

            #region Public Functions
            public void TurnPageOn(Page _page)
            {
                if(_page == null)
                {
                    return;
                }
                canClosePage = false;
                _page.gameObject.SetActive(true);
                _page.Animate(true, this);
            }

            public void TurnPageOff(Page _pageOff, Page _pageOn = null, bool _waitForExit = false)
            {
                if (_pageOff.gameObject.activeSelf)
                {
                    _pageOff.Animate(false, this);
                }

                if (_waitForExit && _pageOff.useAnimation)
                {
                    StopCoroutine("WaitForPageExit");
                    StartCoroutine(WaitForPageExit(_pageOn, _pageOff));
                }
                else
                {
                    TurnPageOn(_pageOn);
                }
            }

            /// <summary>
            /// Turn on pages with type '_type'
            /// </summary>
            public void TurnTypePagesOn(PageType _type) 
            {
                if (_type == PageType.None) return;
                if (!PageExists(_type)) 
                {
                    LogWarning("You are trying to turn a page on ["+_type+"] that has not been registered.");
                    return;
                }

                List<Page> _pages = GetPages(_type);
                for (int i = 0; i < _pages.Count; i++)
                {
                    TurnPageOn(_pages[i]);
                }
               
            }

            /// <summary>
            /// Turn off pages with type '_off'
            /// Optionally turn page with type '_on' on
            /// Optionally wait for page to exit before turning on optional page
            /// </summary>
            public void TurnTypePagesOff(PageType _off, PageType _on=PageType.None, bool _waitForExit=false) 
            {
                if (_off == PageType.None) return;
                if (!PageExists(_off)) 
                {
                    LogWarning("You are trying to turn a page off ["+_on+"] that has not been registered.");
                    return;
                }

                List<Page> _offPages = GetPages(_off);
                for (int i = 0; i < _offPages.Count; i++)
                {
                    if (_offPages[i].gameObject.activeSelf)
                    {
                        _offPages[i].Animate(false, this);
                    }

                    if (_waitForExit && _offPages[i].useAnimation)
                    {
                        List<Page> _onPages = GetPages(_on);
                        for (int j = 0; j < _onPages.Count; j++)
                        {
                            StopCoroutine("WaitForPageExit");
                            StartCoroutine(WaitForPageExit(_onPages[j], _offPages[i]));
                        }
                    }
                    else
                    {
                        TurnTypePagesOn(_on);
                    }
                }
                
            }

            public bool PageIsOn(PageType _type) {
                if (!PageExists(_type)) {
                    LogWarning("You are trying to detect if a page is on ["+_type+"], but it has not been registered.");
                    return false;
                }

                List<Page> _pages = GetPages(_type);
                for (int i = 0; i < _pages.Count; i++)
                {
                    if(_pages[i].type == _type)
                    {
                        return _pages[i].isOn;
                    }
                }

                return false;
            }

            public void SetCanClosePage(bool value)
            {
                Log("SetCanClosePage");
                canClosePage = value;
            }
#endregion

#region Private Functions
            private IEnumerator WaitForPageExit(Page _on, Page _off) 
            {
                while (_off.targetState != Page.FLAG_NONE) 
                {
                    yield return null;
                }

                TurnPageOn(_on);
            }

            private void RegisterAllPages() 
            {
                foreach(Page _page in pages) 
                {
                    RegisterPage(_page);
                }
            }

            private void RegisterPage(Page _page) 
            {
                if (PageExists(_page.type)) 
                {
                    LogWarning("You are trying to register a page ["+_page.type+"] that has already been registered: <color=#f00>"+_page.gameObject.name+"</color>.");
                    return;
                }
                
                m_Pages.Add(_page.type, _page);
                Log("Registered new page ["+_page.type+"].");
            }

            private List<Page> GetPages(PageType _type) 
            {
                if (!PageExists(_type)) 
                {
                    LogWarning("You are trying to get a page ["+_type+"] that has not been registered.");
                    return null;
                }

                List<Page> _pages = new List<Page>();
                for (int i = 0; i < pages.Length; i++)
                {
                    Page page = pages[i];
                    
                    if(page.type == _type)
                    {
                        _pages.Add(page);
                        Debug.Log(page);
                    }
                }

                return _pages;
            }

            private bool PageExists(PageType _type) 
            {
                return m_Pages.ContainsKey(_type);
            }

            private void Log(string _msg) 
            {
                if (!debug) return;
                Debug.Log("[Page Controller]: "+_msg);
            }

            private void LogWarning(string _msg) 
            {
                if (!debug) return;
                Debug.LogWarning("[Page Controller]: "+_msg);
            }
#endregion
        }
    }
}