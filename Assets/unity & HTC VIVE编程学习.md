# unity & HTC VIVE编程学习



### HTC VIVE的VR插件

#### StreamVR plugin



#### Vive Input Utility （基于StreamVR plugin）

##### 1. prefab : 

**ViveCameraRig** = VR devices = HMD(Camera) + controllers + trackers (0.Tutorial)

**VivePointers** = 2 event raycasters that can interact with the UI elements（直线投射光线，操作UI界面）(0.Tutorial)

**ViveCurvePointers** 曲线投射光线，实现瞬移的光线投射。 (4.Teleport)

**ViveColliders**

**ViveRig**

**HandPointers**

**TouchPointers**





如何同时和谐的使用ViveCurvePointers和VivePointers？即在瞬移时指向地面时弧线，而在操作UI时指向Canvas是直线，对ViveCurvePointers中所挂的组件ViveInputVirtualButton增加两个事件，当On Vitual Press Down触发时激活ViveCurvePointers的right和left，当On Vitual Press Up触发时激活VivePointers的right和left。



##### 2. Component （使用C# script实现）:

+ **CanvasRaycastTarget**       接受光线投射并相应 receive raycast and respond (0.Tutorial)     to  Canvas(need image component) / to **Dropdown-Template** （需要对Template添加Canvas和CanvasRaycastTarget组件）；与VivePointers结合使用。
+ **Overlay Keyboard Sample**     **显示键盘用于输入text**；与VivePointers结合使用 (1.UGUI)
+ **Vive Pose Tracker**     **追踪设备的位置**，跟随设备的移动 tracker vive position (8.NearFieldHandInteraction)
  + need other component to adjust position and rotation
    + Pose Freezer
    + Pose Stablizer
    + Pose Easer
+ **Teleportable**    实现**瞬移**    to Floor (as cube , plane , canvas )     cube和plane需要含有**Collider**组件（如Box Collider 或者Mesh Collider），而canvas需要有**CanvasRaycastTarget**组件 ；与ViveCurvePointers结合使用 (4.Teleport)
+ **Custom Device Height**  设置调整VR设备的高度
+ **Draggable**     **使用光线投射抓取物体**，拿住物体（按Hair Trigger）或者移近物体（按Touchpad）等 ；必须要有**Collider**组件，与VivePointers结合使用。（无需ViveColliders组件）
+ **BasicGrabbable**    实现**抓取物体**（按手柄为抓取，松手为释放）   to cube ，如要抓取物体，必须要有**Collider**组件，此外，如果希望物体具有一些物理性质，如重力则要加上Rigidbody组件；如果希望两个物体能够被抓取，又能相互接触而不排斥，则不要使用**Rigidbody**组件，或者勾选Collider中的Is Trigger ，Is Trigger 表示是否可穿透；与VivePointers结合使用。 （必须要有ViveColliders组件）(5.ColliderEvent)
  + **StickyGrabbable**    另一种抓取物体的实现（按一次手柄抓取物体，松手不释放，按两次手柄为释放物体）
+ **ViveInputVirtualButton**    
+ **ModeManager** （可以学着改）控制手柄上的按钮开关，重点在于监听事件的使用(10.ControllerTooltips)
  + ViveInput.AddListenerEx(HandRole.LeftHand, ControllerButton.Menu, ButtonEventType.Down, SwitchToMenuMode);
    + 添加监听事件：当HandRole.LeftHand, ControllerButton.Menu, ButtonEventType.Down触发时，执行函数SwitchToMenuMode
  + ViveInput.RemoveListenerEx(HandRole.LeftHand, ControllerButton.Menu, ButtonEventType.Down, SwitchToMenuMode);
    + 移除监听事件
+ 



**3. Interface**

（触发器事件和接收数据定义来自同一个命名空间）

+ **namespace : [UnityEngine.EventSystems](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.html)**    接收的数据：**PointerEventData** eventData(2.DDragDrop)
  + IBeginDragHandler
  + IDragHandler
  + IEndDragHandler
  + IDropHandler
  + IPointerEnterHandler
  + IPointerExitHandler
+ **namespace : HTC.UnityPlugin.ColliderEvent**       接收的数据：**ColliderButtonEventData** eventData (5.ColliderEvent)
  + IColliderEventPressEnterHandler    在碰撞事件上按着Handler进入：按着触发器（ColliderButtonEventData.InputButton activeButton）进入触发空间时，触发事件
  + IColliderEventPressExitHandler    在碰撞事件上松开Handler退出：松开触发器时退出，触发事件
  + IColliderEventHoverEnterHandler     在碰撞事件上悬停进入
+ 



[SerializeField]







### 语法

+ [【Unity】协程Coroutine及Yield常见用法](https://www.cnblogs.com/guxin/p/unity-how-to-use-coroutine-and-yield.html)
  + yield break; // 直接跳出协程
  + yield return null; // 这一帧到此暂停，下一帧再从暂停处继续，常用于循环中
  + yield return new WaitForSeconds(3.0f); // 等待3秒，然后继续从此处开始，常用于做定时器
  + yield return StartCoroutine(methodName); // 等待另一个协程执行完。这是把协程串联起来的关键，常用于让多个协程按顺序逐个运行
  + yield return new WaitForEndOfFrame(); // 等到这一帧的cameras和GUI渲染结束后再从此处继续，即等到这帧的末尾再往下运行。这行之后的代码还是在当前帧运行，是在下一帧开始前执行，跟return null很相似
  + yield return new WaitForFixedUpdate(); // 在下一次执行FixedUpdate的时候继续执行这段代码，即等一次物理引擎的更新
  + yield return www; // 等待直至异步下载完成

+ C# 函数参数传递    [C#基础：ref和out的区别](https://www.cnblogs.com/gjahead/archive/2008/02/28/1084871.html)
  + 使用ref型参数时，传入的参数必须先被初始化。对out而言，必须在方法中对其完成初始化。
  + 使用ref和out时，在方法的参数和执行方法时，都要加Ref或Out关键字。以满足匹配。
  + 无法定义仅在 ref 和 out 方面不同的重载。
  + 函数的参数传递有四种类型：**传值（by value），传址（by reference），输出参数（by output），数组参数（by array）**。传值参数无需额外的修饰符，传址参数需要修饰符**ref**，输出参数需要修饰符**out**，数组参数需要修饰符**params**。传值参数在方法调用过程中如果改变了参数的值，那么传入方法的参数在方法调用完成以后并不因此而改变，而是保留原来传入时的值。传址参数恰恰相反，如果方法调用过程改变了参数的值，那么传入方法的参数在调用完成以后也随之改变。实际上从名称上我们可以清楚地看出两者的含义--传值参数传递的是调用参数的一份**拷贝**，而传址参数传递的是调用参数的**内存地址**，该参数在方法内外指向的是同一个存储位置。
  + ref    in    out





+ 创建实例和销毁
  + GameObject gameObject = Object.Instantiate(Object original, Transform parent);
  + Destroy(gameObject,0.5f);延迟0.5s后销毁
+ 
