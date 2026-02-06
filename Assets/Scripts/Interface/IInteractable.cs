public interface IInteractable
{
    ItemType ItemType { get; }

    void OnHover();  // 마우스를 오브젝트 위에 가져갔을 때

    void OnHoverExit();

    void OnInteract();  // 상호작용 키를 눌렀을 때
}
