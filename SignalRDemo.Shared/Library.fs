namespace SignalRDemo.Shared

type Id = int

type ClientToServer =
    | GetNotifications

type ServerToClient =
    | Something
