using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;

namespace NCacheClient;

public class PubSubClient : NCache
{
    public PubSubClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {

    }

    public PubSubClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    private const string _orderTopic = "OrderTopic";

    private const string _subscriptionName = "Client1";

    private readonly string[] _allTopics = {
        "Kumail",
        "Ali",
        "OC"
    };

    public override void Test()
    {
        string subTopicName = "OC";
        string pubTopicName = "SKTopic";
        CreateTopic(pubTopicName);
        SubscribeTopic(subTopicName);
        PublishOnTopic(pubTopicName, "Hello, from SK World!");
        CreateTopic(_orderTopic);
        //PublishOnTopic(_durableTopic, "Hello DurableTopic, from SK World! 6");
        //Thread.Sleep(10000);
        //PublishOnTopic(_durableTopic, "Hello DurableTopic, from SK World! 7");
        
        // DummyContinousPublish(_orderTopic);
        // DurableSubsription(_durableTopic);
        // SubscribeTopics(_allTopics);
        // DurableSubscribeTopics(_allTopics, _subscriptionName);
        TestMultipleExclusiveSubscriptions(_orderTopic, _subscriptionName);
    }

    /// <summary>
    /// Test multiple exclusive subscriptions on the same topic, with same subscription name.
    /// This should throw an exception, as multiple exclusive subscriptions, with same subscription name are not allowed on the same topic
    /// </summary>
    public void TestMultipleExclusiveSubscriptions(string topicName, string subscriptionName)
    {
        bool isShared = false;
        DurableSubsription(topicName, subscriptionName, isShared);
        DurableSubsription(topicName, subscriptionName, isShared);
    }

    public void SubscribeTopics(string[] topics)
    {
        foreach (var topic in topics)
        {
            SubscribeTopic(topic);
        }
    }

    public void DurableSubscribeTopics(string[] topics, string subscriptionName, bool isShared = true)
    {
        foreach (var topic in topics)
        {
            DurableSubsription(topic, subscriptionName, isShared);
        }
    }

    public void DummyContinousPublish(string topic)
    {
        long messageId = 1;//long.MinValue;
        log.Debug($"Publishing messages on topic: [{topic}] on cache: [{_cacheName}]");
        while (true)
        {
            try
            {
                PublishOnTopic(topic, $"{++messageId} Hello World!");
                if (messageId >= long.MaxValue)
                {
                    break;
                }
                System.Threading.Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                log.Error($"Error publishing message: {ex.Message}");
            }
        }
    }

    public void CreateTopic(string topicName)
    {
        try
        {
            if (cache.MessagingService.GetTopic(topicName) != null)
            {
                log.Debug($"Topic already exists: {topicName}, on cache: {_cacheName}");
            }
            else
            {
                // Create the topic
                ITopic topic = cache.MessagingService.CreateTopic(topicName);
                log.Debug($"Topic created: {topicName}, on cache: {_cacheName}");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Error creating topic: {ex.Message}");
        }
    }
    public void PublishOnTopic(string topicName, string message, TimeSpan? timeSpan = null)
    {
        // Get the topic
        ITopic topic = cache.MessagingService.GetTopic(topicName);

        if (topic != null)
        {
            // Create the new message with the object order
            var orderMessage = new Message(message);

            // Set the expiration time of the message
            orderMessage.ExpirationTime = timeSpan ?? TimeSpan.FromSeconds(5000);

            // Register message delivery failure
            topic.MessageDeliveryFailure += OnFailureMessageReceived;

            //Register topic deletion notification
            topic.OnTopicDeleted = TopicDeleted;

            // Publishing the message with delivery option set as all
            // also register message delivery failure
            bool notifyDeliveryFailure = true;
            topic.Publish(orderMessage, DeliveryOption.All, notifyDeliveryFailure);
            log.Debug($"Published message: [{message}] on topic: [{topicName}] cache: {_cacheName}");
        }
        else
        {
            log.Error($"Topic not found: {topicName}, on cache: {_cacheName}, Are you sure you have created the topic?");
        }
    }

    public void SubscribeTopic(string topicName)
    {
        // Get the topic
        ITopic topic = cache.MessagingService.GetTopic(topicName);
        MessageReceivedCallback messageReceivedCallback = new MessageReceivedCallback(MessageReceived);
        // If topic exists, Create subscription
        if (topic != null)
        {
            // Create and register subscribers for order topic
            // Message received callback is specified
            ITopicSubscription subscription = topic.CreateSubscription(messageReceivedCallback);
            log.Debug($"Subscribed topic: {topicName}, on cache: {_cacheName}");
        }
    }

    public void DurableSubsription(string topicName, string subscriptionName, bool isShared = true)
    {
        try
        {
            ITopic topic = cache.MessagingService.GetTopic(topicName);
            // If topic exists, Create subscription
            if (topic != null)
            {
                MessageReceivedCallback durableMessageCallback = new MessageReceivedCallback(DurableMessagReceived);
                SubscriptionPolicy subscriptionPolicy = isShared ? SubscriptionPolicy.Shared : SubscriptionPolicy.Exclusive;
                DeliveryMode deliveryMode = DeliveryMode.Async;
                TimeSpan timeSpan = TimeSpan.FromSeconds(10);
                IDurableTopicSubscription durableSubscription = topic.CreateDurableSubscription(subscriptionName, subscriptionPolicy, durableMessageCallback, timeSpan, deliveryMode);
                log.Debug($"topic: {topicName}, subscribed via subscriptionName: {subscriptionName} with durable subscription, on cache: {_cacheName}");
            }
            else
            {
                log.Debug($"Cannot subscribe to topic: {topicName}, with subscriptionName: {subscriptionName} on cache: {_cacheName}, topic doesn't exist");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Error creating durable subscription topic: {topicName}, subscriptionName: {subscriptionName}: {ex.Message}");
        }
    }

    public void UnsubscribeTopic(string topicName)
    {
        // Get the topic
        ITopic topic = cache.MessagingService.GetTopic(topicName);
        // If topic exists, Unsubscirbe
        if (topic != null)
        {
            // Create and register subscribers for order topic
            // Message received callback is specified
            ITopicSubscription subscription = topic.CreateSubscription(new MessageReceivedCallback(MessageReceived));
            log.Debug($"UnSubscribed topic: {topicName}, on cache: {_cacheName}");
        }
    }

    public void ContinousQuery()
    {
        // Query for required operation
        string query = "SELECT $VALUE$ FROM NCacheClient.Subscriber WHERE Age > ?";

        var queryCommand = new QueryCommand(query);
        queryCommand.Parameters.Add("Country", "USA");

        // Create Continuous Query
        var cQuery = new ContinuousQuery(queryCommand);

        // Register to be notified when a qualified item is added to the cache
        cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallBack), EventType.ItemAdded | EventType.ItemUpdated | EventType.ItemRemoved, EventDataFilter.None);

        // Register continuousQuery on server 
        cache.MessagingService.RegisterCQ(cQuery);
    }

    private void QueryItemCallBack(string key, CQEventArg arg)
    {
        log.Debug($"QueryItemCallBack: key: [{key}], EventType: [{arg.EventType}]");
    }

    public void MessageReceived(object sender, MessageEventArgs args)
    {
        Message message = (Message)args.Message;
        log.Debug($"Message received: [{message.Payload}], topic: [{args.Topic.Name}]");
    }

    public void DurableMessagReceived(object sender, MessageEventArgs args)
    {
        Message message = (Message)args.Message;
        log.Debug($"Durable Message received: [{message.Payload}], topic: [{args.Topic.Name}]");
    }
    private void TopicDeleted(object sender, EventArgs args)
    {
        log.Warn($"Topic deleted: {args}, sender: {sender}");
    }

    private void OnFailureMessageReceived(object sender, MessageFailedEventArgs args)
    {
        log.Error($"Message delivery failed: {args.Message}, topic: {args.Topic} sender: {sender}, cache: {_cacheName}");
    }
}