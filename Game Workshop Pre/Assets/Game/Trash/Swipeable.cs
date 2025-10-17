using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Swipeable
{

    public void OnSwiped();

    public void OnSwipeEnd();

    public IEnumerator SwipedEndCoroutine();


}
