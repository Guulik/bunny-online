using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMember : MonoBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI nicknameText;
    private Texture2D _steamAvatar;

    private string _steamNickname;

    private CSteamID SteamID { get; set; }

    public void Initialize(CSteamID steamID)
    {
        SteamID = steamID;
        _steamNickname = SteamFriends.GetFriendPersonaName(SteamID);
        nicknameText.text = _steamNickname;
        LoadSteamAvatar();
    }

    private void LoadSteamAvatar()
    {
        int avatarID = SteamFriends.GetLargeFriendAvatar(SteamID);
        if (avatarID != -1)
        {
            SteamUtils.GetImageSize(avatarID, out uint width, out uint height);
            byte[] image = new byte[width * height * 4];

            if (SteamUtils.GetImageRGBA(avatarID, image, (int)(width * height * 4)))
            {
                _steamAvatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                _steamAvatar.LoadRawTextureData(image);
                _steamAvatar.Apply();

                _steamAvatar = FlipTextureVertically(_steamAvatar);

                avatarImage.sprite = Sprite.Create(_steamAvatar,
                    new Rect(0, 0, _steamAvatar.width, _steamAvatar.height), new Vector2(0.5f, 0.5f));
            }
        }
        else
        {
            Debug.LogWarning("Failed to load Steam avatar.");
            LoadSteamAvatar();
        }
    }

    private Texture2D FlipTextureVertically(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);

        for (int y = 0; y < original.height; y++)
        for (int x = 0; x < original.width; x++)
            flipped.SetPixel(x, original.height - y - 1, original.GetPixel(x, y));

        flipped.Apply();
        return flipped;
    }
}