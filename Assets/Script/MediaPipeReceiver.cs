using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class CameraReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private int port = 12345;
    private List<byte> imageDataList = new List<byte>();

    void Start()
    {
        udpClient = new UdpClient(port);
        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        byte[] receivedBytes = udpClient.EndReceive(ar, ref remoteEndPoint);

        // 最初にチャンクの長さを受け取る
        int chunkSize = BitConverter.ToInt32(receivedBytes, 0);
        byte[] chunkData = new byte[chunkSize];
        Array.Copy(receivedBytes, 4, chunkData, 0, chunkSize);

        // 受け取ったチャンクデータをリストに追加
        imageDataList.AddRange(chunkData);

        // 完全な画像データが揃った場合、画像を処理
        if (imageDataList.Count >= chunkSize) 
        {
            byte[] completeImageData = imageDataList.ToArray();
            // ここで画像を処理するためのコードを追加
            // 例えば、Unityで画像を表示するためにTexture2Dに変換するなど
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(completeImageData);  // 画像データをTexture2Dに読み込む
            GetComponent<Renderer>().material.mainTexture = texture;

            // 画像データリストをクリア
            imageDataList.Clear();
        }

        // 次の受信を開始
        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
