// JSON�ۑ����ǂݍ���
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;

public static class ScoreSaveSystem
{
    private static readonly string filePath = Path.Combine(Application.persistentDataPath, "score.json");

    public static async UniTask<ScoreData> LoadAsync()
    {
        if (!File.Exists(filePath))
        {
            return new ScoreData { bestScore = 0 };
        }

        using var reader = new StreamReader(filePath);
        var json = await reader.ReadToEndAsync();
        return JsonUtility.FromJson<ScoreData>(json);
    }

    public static async UniTask<bool> SaveIfBestAsync(int currentScore)
    {
        var data = await LoadAsync();

        if (currentScore > data.bestScore)
        {
            data.bestScore = currentScore;

            string json = JsonUtility.ToJson(data, prettyPrint: true);
            using var writer = new StreamWriter(filePath, false);
            await writer.WriteAsync(json);

            return true; // �V�L�^�������ꍇ
        }

        return false; // �L�^�X�V�Ȃ�
    }


    public static async UniTask<int> GetBestScoreAsync()
    {
        var data = await LoadAsync();
        return data.bestScore;
    }
}
