public interface IEnterable
{
    AreaType AreaType { get; }  // 지역의 타입
    void OnPlayerEnter();  // 플레이어가 지역에 들어갈 때
    void OnPlayerExit();  // 플레이어가 지역에서 나갈 때
}