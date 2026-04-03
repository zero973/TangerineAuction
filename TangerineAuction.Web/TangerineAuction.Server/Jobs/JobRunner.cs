using Hangfire;
using System.Linq.Expressions;
using System.Reflection;
using TangerineAuction.Core.Services;
using TangerineAuction.Shared.Hangfire;

namespace TangerineAuction.Server.Jobs;

internal class JobRunner(IEnumerable<IRecurringJob> jobs) : IJobRunner
{
    private static readonly MethodInfo AddOrUpdateOpenGenericMethod =
        typeof(RecurringJob)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
            {
                if (m.Name != nameof(RecurringJob.AddOrUpdate) || !m.IsGenericMethodDefinition)
                    return false;

                var parameters = m.GetParameters();
                if (parameters.Length != 3)
                    return false;

                if (parameters[0].ParameterType != typeof(string) ||
                    parameters[2].ParameterType != typeof(string))
                    return false;

                var secondParameterType = parameters[1].ParameterType;

                return secondParameterType.IsGenericType
                       && secondParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
                       && secondParameterType.GenericTypeArguments[0].IsGenericType
                       && secondParameterType.GenericTypeArguments[0].GetGenericTypeDefinition() == typeof(Func<,>)
                       && secondParameterType.GenericTypeArguments[0].GenericTypeArguments[1] == typeof(Task);
            });

    public void Run()
    {
        var jobTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IRecurringJob).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        foreach (var jobType in jobTypes)
        {
            var job = jobs.Single(j => j.GetType() == jobType);

            var closedMethod = AddOrUpdateOpenGenericMethod.MakeGenericMethod(jobType);

            closedMethod.Invoke(null, [
                job.Name,
                CreateExpression(jobType),
                job.Cron
            ]);
        }
    }

    private static LambdaExpression CreateExpression(Type jobType)
    {
        var executeAsyncMethod = jobType.GetMethod(
            nameof(IRecurringJob.ExecuteAsync),
            BindingFlags.Public | BindingFlags.Instance);

        if (executeAsyncMethod is null)
        {
            throw new InvalidOperationException(
                $"Method '{nameof(IRecurringJob.ExecuteAsync)}' was not found on job type '{jobType.FullName}'.");
        }

        var parameter = Expression.Parameter(jobType, "job");
        var call = Expression.Call(parameter, executeAsyncMethod);

        var delegateType = typeof(Func<,>).MakeGenericType(jobType, typeof(Task));

        return Expression.Lambda(delegateType, call, parameter);
    }
}