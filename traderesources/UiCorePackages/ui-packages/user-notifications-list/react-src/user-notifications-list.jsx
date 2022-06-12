﻿
class UserNotificationList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            notifications: props.notifications,
            currentNotification: null,
            notificationBellBadge: null
        };
        this.loadMoreIfNeed = this.loadMoreIfNeed.bind(this);
        this.loadMore = this.loadMore.bind(this);
        this.handleNotificationClick = this.handleNotificationClick.bind(this);
    }

    componentWillMount() {
        window.addEventListener('scroll', this.loadMoreIfNeed);
        this.loadMore();
        this.notificationBellBadge = $("#notification-bell-badge"); 
    }

    componentWillUnmount() {
        window.removeEventListener('scroll', this.loadMoreIfNeed);
    }

    loadMoreIfNeed() {
        if (window.innerHeight + document.documentElement.scrollTop === document.scrollingElement.scrollHeight) {
            this.loadMore();
        }
    }

    loadMore() {
        var lastTimeStamp = this.state.notifications.length > 0 ? this.state.notifications[this.state.notifications.length - 1].TimeStamp : null;

        $.post(
            this.props.urlToLoadOldest,
            {
                FromDateStrInIso8601: lastTimeStamp,
                Count: this.props.loadOldestCount,
            },
            (newLoadednotifications) => {
                this.setState(prevState => ({
                    notifications: prevState.notifications.concat(newLoadednotifications)
                }))
            }
        );
    }

    handleNotificationClick(notification) {
        //меняем знаяение не прчитаггых собщений возле колокольчика
        if (!notification.IsReaded) {
            var newUnreadNotificationsCount = (parseInt(this.notificationBellBadge.text()) + 0) - 1;
            if (newUnreadNotificationsCount > 0) {
                this.notificationBellBadge.text(newUnreadNotificationsCount);
            } else {
                this.notificationBellBadge.text("");
            }
        }

        //асинхронный запрос на отметку данного уведомления как 'прочитанно'
        $.post(this.props.urlToMarkAsRead, { NotificationId: notification.Id});

        //обновляем ui
        var index = this.state.notifications.findIndex(x => x.Id === notification.Id);
        var updatedNotification = Object.assign({}, this.state.notifications[index]);
        updatedNotification.IsReaded = true;

        this.setState(prevState => ({
            currentNotification: notification,
            notifications: prevState.notifications.slice(0, index).concat(updatedNotification).concat(prevState.notifications.slice(index + 1))
        }));
    }

    render() {
        if(this.state.notifications.length == 0){
            return (<h1>Нет сообщений</h1>);
        }
        var notificationsItems = this.state.notifications.map(x => (
            <div className="card" key={x.Id}>
                <div className="card-body">
                    <div className="media">
                        <div className="avatar-sm">
                            <span className={x.IsReaded ? "avatar-title bg-secondary rounded-circle" : "avatar-title bg-primary rounded-circle"}>
                                <i className="uil uil-envelope text-white font-24"></i>
                            </span>
                        </div>
                        <div className="media-body pl-2 pt-1">
                            <div>
                                <span className="float-right pl-1 text-dark">{x.TimeStampPrettyStr}</span>
                                <a className={x.IsReaded ? "mr-1 text-dark" : "mr-1 text-primary"} onClick={() => this.handleNotificationClick(x)} data-toggle="modal" data-target="#scrollable-modal"><b className="">{x.Msg.Title}</b></a> <span class="badge badge-dark font-12">{x.Channel.PrettyName}</span>
                            </div>

                            <span>{x.Msg.Body}</span>

                            <div className="float-right">
                                {
                                    x.Msg.Tags.map(function (tag) {
                                        return <span className="badge badge-outline-primary float-right ml-1 mt-1">{tag}</span>
                                    })
                                }
                            </div>
                        </div>
                        <hr/>
                    </div>
                </div>
            </div>
            ));

        return (
        <div>
            {notificationsItems}
            <div class="modal fade" id="scrollable-modal" tabindex="-1" role="dialog" aria-labelledby="scrollableModalTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-scrollable" role="document">
                    <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="scrollableModalTitle">{this.state.currentNotification === null ? "Nope" : this.state.currentNotification.Msg.Title}</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                                <div class="text-dark mb-2"><b>
                                    {this.state.currentNotification === null ? "Nope" : new Date(this.state.currentNotification.TimeStamp).toLocaleDateString("ru-RU") + ", " + new Date(this.state.currentNotification.TimeStamp).toLocaleTimeString("ru-RU")}
                                </b></div>
                                {this.state.currentNotification === null ? "Nope" : this.state.currentNotification.Msg.Body}
                        </div>
                            <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Закрыть</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>);
    };
}


$(document).ready(function () {
    $(".user-notifications-list").each(function () {
        const $this = $(this);
        const editorName = $this.data("editorName");
        const notifications = $this.data("notifications");
        const urlToLoadOldest = $this.data("urlToLoadOldest");
        const loadOldestCount = $this.data("loadOldestCount");
        const urlToMarkAsRead = $this.data("urlToMarkAsRead");
        ReactDOM.render(
            <UserNotificationList name={editorName} notifications={notifications} urlToLoadOldest={urlToLoadOldest} loadOldestCount={loadOldestCount} urlToMarkAsRead={urlToMarkAsRead} />,
            this
        );
    });
  
});
