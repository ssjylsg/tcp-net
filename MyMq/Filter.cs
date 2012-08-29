using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MyMq
{
    internal class TcpClientEndPoint
    {
        private EndPoint _point;
        private TcpClient _client;

        public EndPoint Point
        {
            get { return _point; }
            set { _point = value; }
        }

        public TcpClient Client
        {
            get { return _client; }
            set { _client = value; }
        }
    }

    internal interface IClientRepository<T>
    {
        void AddSubscriber(String topicName, T subscriberEndPoint);
        List<T> GetSubscribers(String topicName);
        void RemoveSubscriber(String topicName, T subscriberEndPoint);
    }

    internal class ClientRepository<T> : IClientRepository<T>
    {
        private Dictionary<string, List<T>> _subscribersList;
        public Dictionary<string, List<T>> SubscribersList
        {
            get
            {
                lock (_lockObject)
                {
                    return _subscribersList;
                }
            }
        }
        private object _lockObject = new object();
        public ClientRepository()
        {
            _subscribersList = new Dictionary<string, List<T>>();
        }
        public void AddSubscriber(String topicName, T subscriberEndPoint)
        {
            lock (_lockObject)
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    if (!SubscribersList[topicName].Contains(subscriberEndPoint))
                    {
                        SubscribersList[topicName].Add(subscriberEndPoint);
                    }
                }
                else
                {
                    List<T> newSubscribersList = new List<T>();
                    newSubscribersList.Add(subscriberEndPoint);
                    SubscribersList.Add(topicName, newSubscribersList);
                }
            }
        }
        public List<T> GetSubscribers(String topicName)
        {
            lock (_lockObject)
            {
                return SubscribersList.ContainsKey(topicName) ? SubscribersList[topicName] : null;
            }
        }
        public void RemoveSubscriber(String topicName, T subscriberEndPoint)
        {
            lock (_lockObject)
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    if (SubscribersList[topicName].Contains(subscriberEndPoint))
                    {
                        SubscribersList[topicName].Remove(subscriberEndPoint);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 维护订阅者
    /// </summary>
    internal class Filter
    {
      //  public static TcpClient Client;
        /// <summary>
        /// 订阅者数据字典
        /// </summary>
        static Dictionary<string, List<EndPoint>> _subscribersList = new Dictionary<string, List<EndPoint>>();
        static ClientRepository<TcpClient> _clientRepository = new ClientRepository<TcpClient>();
        private static object _lockObject = new object();
        public static ClientRepository<TcpClient> ClientRepository
        {
            get { return _clientRepository; }
        }
        /// <summary>
        /// 订阅者数据字典
        /// </summary>
        public static Dictionary<string, List<EndPoint>> SubscribersList
        {
            get
            {
                lock (_lockObject)
                {
                    return _subscribersList;
                }
            }
        }
        /// <summary>
        /// 根据主题获取订阅者
        /// </summary>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public static List<EndPoint> GetSubscribers(String topicName)
        {
            lock (_lockObject)
            {
                return SubscribersList.ContainsKey(topicName) ? SubscribersList[topicName] : new List<EndPoint>();
            }
        }
        /// <summary>
        /// 增加订阅者
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="subscriberEndPoint"></param>
        public static void AddSubscriber(String topicName, EndPoint subscriberEndPoint)
        {
            lock (_lockObject)
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    if (!SubscribersList[topicName].Contains(subscriberEndPoint))
                    {
                        SubscribersList[topicName].Add(subscriberEndPoint);
                    }
                }
                else
                {
                    List<EndPoint> newSubscribersList = new List<EndPoint>();
                    newSubscribersList.Add(subscriberEndPoint);
                    SubscribersList.Add(topicName, newSubscribersList);
                }
            }
        }
        /// <summary>
        /// 取消订阅者
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="subscriberEndPoint"></param>
        public static void RemoveSubscriber(String topicName, EndPoint subscriberEndPoint)
        {
            lock (_lockObject)
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    if (SubscribersList[topicName].Contains(subscriberEndPoint))
                    {
                        SubscribersList[topicName].Remove(subscriberEndPoint);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 保存订阅者 【订阅者EndPoint + TCPClient】
    /// </summary>
    internal class TcpClientFilter
    {
       // public static TcpClient Client;
        /// <summary>
        /// 订阅者数据字典
        /// </summary>
        static Dictionary<string, List<TcpClientEndPoint>> _subscribersList = new Dictionary<string, List<TcpClientEndPoint>>();
        private static object _lockObject = new object();

        /// <summary>
        /// 订阅者数据字典
        /// </summary>
        public static Dictionary<string, List<TcpClientEndPoint>> SubscribersList
        {
            get
            {
                lock (_lockObject)
                {
                    return _subscribersList;
                }
            }
        }
        /// <summary>
        /// 根据主题获取订阅者
        /// </summary>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public static List<TcpClient> GetSubscribers(String topicName)
        {
            lock (_lockObject)
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    List<TcpClient> list = new List<TcpClient>();
                    foreach (TcpClientEndPoint tcpClientEndPoint in SubscribersList[topicName])
                    {
                        if (tcpClientEndPoint.Client.Connected)
                        {
                            list.Add(tcpClientEndPoint.Client);
                        }
                    }
                    return list;
                }
                return new List<TcpClient>();
            }
        }
        /// <summary>
        /// 查找相同的EndPoint 的TcpClient
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sPoint"></param>
        /// <param name="findPoint"></param>
        /// <returns></returns>
        private static bool FindFirstSameEndPoint(List<TcpClientEndPoint> list, EndPoint sPoint,out TcpClientEndPoint findPoint)
        {
            foreach (TcpClientEndPoint tcpClientEndPoint in list)
            {
                if (tcpClientEndPoint.Point == sPoint)
                {
                    findPoint = tcpClientEndPoint;
                    return true;
                }
            }
            findPoint = null;
            return false;
        }
        /// <summary>
        /// 增加订阅者
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="subscriberEndPoint"></param>
        public static void AddSubscriber(String topicName, EndPoint subscriberEndPoint, TcpClient client)
        {
            lock (_lockObject)
            {
                TcpClientEndPoint point = new TcpClientEndPoint();
                point.Client = client;
                point.Point = subscriberEndPoint;
                if (SubscribersList.ContainsKey(topicName))
                {
                    TcpClientEndPoint findPoint;
                    if (FindFirstSameEndPoint(SubscribersList[topicName], subscriberEndPoint, out findPoint))
                    {
                        if (findPoint.Client.Connected == false)
                        {
                            SubscribersList[topicName].Remove(findPoint);
                        }
                    }
                    SubscribersList[topicName].Add(point);
                }
                else
                {
                    List<TcpClientEndPoint> newSubscribersList = new List<TcpClientEndPoint>();
                    newSubscribersList.Add(point);
                    SubscribersList.Add(topicName, newSubscribersList);
                }
            }
        }
        /// <summary>
        /// 取消订阅者
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="subscriberEndPoint"></param>
        public static void RemoveSubscriber(String topicName, EndPoint subscriberEndPoint)
        {
            lock (_lockObject)
            {
                if (SubscribersList.ContainsKey(topicName))
                {
                    TcpClientEndPoint point;
                    if (FindFirstSameEndPoint(SubscribersList[topicName], subscriberEndPoint, out point))
                    {
                        SubscribersList[topicName].Remove(point);
                    }
                }
            }
        }
    }
}
