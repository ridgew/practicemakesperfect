//Excel中RATE函数的Java实现
//https://www.cnblogs.com/growithus/p/11012168.html

public class RATE {

    /**
     * calculateRate:类excel中的RATE函数，计算结果值为月利率，年华利率 需*12期. <br/>
     * rate = calculateRate(periods, payment, present_val, future_val, type,
     * estimate) ;
     * 
     * @author guooo 2018年7月11日 上午11:13:55
     * @param nper
     *            为总投资期，即该项投资的付款期总数。
     * @param pmt
     *            为各期付款额，其数值在整个投资期内保持不变。通常 pmt 包括本金和利息，但不包括其他费用或税金。如果忽略了
     *            pmt，则必须包含 fv 参数。
     * @param pv
     *            为现值，即从该项投资开始计算时已经入帐的款项，或一系列未来付款当前值的累积和，也称为本金。
     * @param fv
     *            为未来值，或在最后一次付款后希望得到的现金余额，如果省略 fv，则假设其值为零，也就是一笔贷款的未来值为零。
     * @param type
     *            数字 0 或 1，用以指定各期的付款时间是在期初还是期末。 0或省略-期末|| 1-期初
     * @param guess
     *            预期利率。 如果省略预期利率，则假设该值为 10%。
     * @return
     * @since JDK 1.6
     */
    public static double calculateRate(double nper, double pmt, double pv, double fv, double type, double guess) {
        //FROM MS http://office.microsoft.com/en-us/excel-help/rate-HP005209232.aspx
        int FINANCIAL_MAX_ITERATIONS = 20;//Bet accuracy with 128
        double FINANCIAL_PRECISION = 0.0000001;//1.0e-8

        double y, y0, y1, x0, x1 = 0, f = 0, i = 0;
        double rate = guess;
        if (Math.abs(rate) < FINANCIAL_PRECISION) {
            y = pv * (1 + nper * rate) + pmt * (1 + rate * type) * nper + fv;
        } else {
            f = Math.exp(nper * Math.log(1 + rate));
            y = pv * f + pmt * (1 / rate + type) * (f - 1) + fv;
        }
        y0 = pv + pmt * nper + fv;
        y1 = pv * f + pmt * (1 / rate + type) * (f - 1) + fv;

        // find root by Newton secant method
        i = x0 = 0.0;
        x1 = rate;
        while ((Math.abs(y0 - y1) > FINANCIAL_PRECISION) && (i < FINANCIAL_MAX_ITERATIONS)) {
            rate = (y1 * x0 - y0 * x1) / (y1 - y0);
            x0 = x1;
            x1 = rate;

            if (Math.abs(rate) < FINANCIAL_PRECISION) {
                y = pv * (1 + nper * rate) + pmt * (1 + rate * type) * nper + fv;
            } else {
                f = Math.exp(nper * Math.log(1 + rate));
                y = pv * f + pmt * (1 / rate + type) * (f - 1) + fv;
            }

            y0 = y1;
            y1 = y;
            ++i;
        }
        return rate;
    }

    /**
     * simpleCalculateRate:(这里用一句话描述这个方法的作用). <br/>
     *
     * @author guooo 2018年7月12日 上午11:19:24
     * @param nper
     *            为总投资期，即该项投资的付款期总数。
     * @param pmt
     *            为各期付款额，其数值在整个投资期内保持不变。通常 pmt 包括本金和利息，但不包括其他费用或税金。如果忽略了
     *            pmt，则必须包含 fv 参数。
     * @param pv
     *            为现值，即从该项投资开始计算时已经入帐的款项，或一系列未来付款当前值的累积和，也称为本金。
     * @return
     * @since JDK 1.6
     */
    public static double simpleCalculateRate(double nper, double pmt, double pv) {

        double fv = 0;

        //0或省略-期末支付
        double type = 0;

        //如果省略预期利率，则假设该值为 10%。
        double guess = 0.1;

        return calculateRate(nper, pmt, pv, fv, type, guess);
    }

    public static void main(String[] args) {
        System.out.println(simpleCalculateRate(12, 874.52, -10000) * 12);
        System.out.println(simpleCalculateRate(24, 454.67, -10000) * 12);
        System.out.println(simpleCalculateRate(36, 315.67, -10000) * 12);
    }
}
