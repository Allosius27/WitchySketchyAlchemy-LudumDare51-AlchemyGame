//
// Updated by Allosius(Yanis Q.) on 7/9/2022.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AllosiusDevUtilities.Core
{

    namespace Menu {

        public class TestMenu : MonoBehaviour
        {
            public PageController pageController;

 #if UNITY_EDITOR
            private void Update() {
                if (Input.GetKeyUp(KeyCode.F)) {
                    pageController.TurnTypePagesOn(PageType.Loading);
                }
                if (Input.GetKeyUp(KeyCode.G)) {
                    pageController.TurnTypePagesOff(PageType.Loading);
                }
                if (Input.GetKeyUp(KeyCode.H)) {
                    pageController.TurnTypePagesOff(PageType.Loading, PageType.Menu);
                }
                if (Input.GetKeyUp(KeyCode.J)) {
                    pageController.TurnTypePagesOff(PageType.Loading, PageType.Menu, true);
                }
            }

            [Button(ButtonSizes.Large)]
            public void TurnPageOn(Page page)
            {
                pageController.TurnPageOn(page);
            }

            [Button(ButtonSizes.Large)]
            public void TurnPageOff(Page page)
            {
                pageController.TurnPageOff(page);
            }

            [Button(ButtonSizes.Large)]
            public void TurnPageOnAndOff(Page pageOff, Page pageOn)
            {
                pageController.TurnPageOff(pageOff, pageOn);
            }

            [Button(ButtonSizes.Large)]
            public void TurnTypePageOn(PageType type)
            {
                pageController.TurnTypePagesOn(type);
            }

            [Button(ButtonSizes.Large)]
            public void TurnTypePageOff(PageType type)
            {
                pageController.TurnTypePagesOff(type);
            }

            [Button(ButtonSizes.Large)]
            public void TurnTypePageOffAndOn(PageType typeOff, PageType typeOn)
            {
                pageController.TurnTypePagesOff(typeOff, typeOn);
            }
#endif           
        }
    }
}
