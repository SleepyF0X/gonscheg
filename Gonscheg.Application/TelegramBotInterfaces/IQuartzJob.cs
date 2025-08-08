using Quartz;

namespace Gonscheg.Application.TelegramBotInterfaces;

public interface IQuartzJob : IJob
{
    public static abstract string CRONTime { get; }
}
