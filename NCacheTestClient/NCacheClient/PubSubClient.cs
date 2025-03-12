using Alachisoft.NCache.Runtime.Caching;

namespace NCacheClient;

public class PubSubClient : NCache
{
    public PubSubClient(string ip, int port, string cacheName) : base(ip, port, cacheName)
    {

    }

    public PubSubClient(List<string> ips, int port, string cacheName) : base(ips, port, cacheName)
    {
    }

    public override void Test()
    {
        string subTopicName = "OC";
        string pubTopicName = "RamadanKareemBro33333333333333";
        CreateTopic(pubTopicName);
        SubscribeTopic(subTopicName);
        long messageId = long.MinValue;
        log.Debug($"Publishing messages on topic: {pubTopicName} cache: {_cacheName}");
        while (true)
        {
            try
            {
                PublishOnTopic(pubTopicName, $"{++messageId} Hello World!");
                if (messageId <= long.MaxValue)
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
    public void PublishOnTopic(string topicName, string message)
    {
        // Get the topic
        ITopic orderTopic = cache.MessagingService.GetTopic(topicName);

        if (orderTopic != null)
        {
            // Create the object to be sent in message
            //Subscriber sub = new Subscriber() { Msisdn = "1234567890", Id = 1 };

            // Create the new message with the object order
            var orderMessage = new Message(message);

            // Set the expiration time of the message
            orderMessage.ExpirationTime = TimeSpan.FromSeconds(5000);

            // Register message delivery failure
            orderTopic.MessageDeliveryFailure += OnFailureMessageReceived;

            //Register topic deletion notification
            orderTopic.OnTopicDeleted = TopicDeleted;

            // Publish the order with delivery option set as all
            // and register message delivery failure
            orderTopic.Publish(orderMessage, DeliveryOption.All, true);
            log.Debug($"Published message: {message} on topic: {topicName} cache: {_cacheName}");
        }
        else
        {
            log.Error($"Topic not found: {topicName}, on cache: {_cacheName}");
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

    public void MessageReceived(object sender, MessageEventArgs args)
    {
        Message message = (Message)args.Message;
        log.Debug($"Message received: [{message.Payload}], topic: [{args.Topic.Name}]");
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