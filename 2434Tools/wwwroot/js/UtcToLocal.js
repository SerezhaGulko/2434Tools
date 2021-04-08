var DateNow = new Date();
const Divisors =
    [1000 * 3600 * 24 * 365.2422, 1000 * 3600 * 24 * 30.42, 1000 * 3600 * 24 * 7,
    1000 * 3600 * 24, 1000 * 3600, 1000 * 60]
$('.video-date').each((idx, elem) => {
    var LocalDate = new Date(Date.UTC(...elem.innerText.split(' ')));
    var DateDiff = DateNow - LocalDate;
    var AbsDiff = Math.abs(DateDiff);
    [Years, Months, Weeks, Days, Hours, Minutes]
        = [AbsDiff, AbsDiff, AbsDiff, AbsDiff, AbsDiff, AbsDiff].map((val, idx) => {
            return Math.floor(val / Divisors[idx]);
        });
    n = "";
    // Can be simplified
    if (Years >= 1) {
        n = `${Years} ${Years == 1 ? "year" : "years"}`
    } else if (Months >= 1) {
        n = `${Months} ${Months == 1 ? "month" : "months"}`
    } else if (Weeks >= 1) {
        n = `${Weeks} ${Weeks == 1 ? "week" : "weeks"}`
    } else if (Days >= 1) {
        n = `${Days} ${Days == 1 ? "day" : "days"}`
    } else if (Hours >= 1) {
        n = `${Hours} ${Hours == 1 ? "hour" : "hours"}`
    } else if (Minutes >= 1) {
        n = `${Minutes} ${Minutes == 1 ? "minute" : "minutes"}`
    } else {
        n = "Just a few seconds";
    }
    if (DateDiff < 0) {
        elem.innerText = "Live in " + n;
    } else {
        elem.innerText = n + " ago"
    }
})
