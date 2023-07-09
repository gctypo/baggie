namespace Baggie

type IAppConfigProvider =
    abstract member GetConfigValue : key: string -> string
