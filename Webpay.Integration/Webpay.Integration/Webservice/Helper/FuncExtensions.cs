namespace Webpay.Integration.Webservice.Helper;

public static class FuncExtensions
{
    public static Maybe<T> ToMaybe<T>(this T value)
    {
        return new Maybe<T>(value);
    }

    public static Maybe<U> SelectMany<T, U>(this Maybe<T> m, Func<T, Maybe<U>> k)
    {
        if (!m.HasValue)
            return Maybe<U>.Nothing;
        return k(m.Value);
    }

    public static Maybe<T> If<T>(this T val)
    {
        return val.ToMaybe();
    }

    public static Maybe<U> And<T, U>(this Maybe<T> val, Func<T, U> func)
    {
        if (!val.HasValue)
            return Maybe<U>.Nothing;
        U retVal;
        try
        {
            retVal = func(val.Value);
            if (retVal == null)
                throw new ArgumentNullException();
        }
        catch
        {
            return Maybe<U>.Nothing;
        }
        return retVal.ToMaybe();
    }

    public static Maybe<U> And<T, U>(this Maybe<T> val, Func<T, U> func, Action<T, System.Exception> errorContinuation)
    {
        if (!val.HasValue)
            return Maybe<U>.Nothing;
        U retVal;
        try
        {
            retVal = func(val.Value);
            if (retVal == null)
                throw new ArgumentNullException();
        }
        catch (System.Exception ex)
        {
            errorContinuation(val.Value, ex);
            return Maybe<U>.Nothing;
        }
        return retVal.ToMaybe();
    }

    public static Maybe<T> FailWith<T>(this Maybe<T> val, Action errorContinuation)
    {
        if (!val.HasValue)
            errorContinuation();
        return val;
    }
}