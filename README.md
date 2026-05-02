# פרופיל סטודנט (Student Profile)

אפליקציית **פרופיל סטודנט** מבוססת מיקרו־שירות: API ב־**ASP.NET Core 7** עם **Entity Framework Core**, ולקוח **React (Vite)** עם **Material UI**. כולל אימות **JWT**, ניהול משתמשים, פרטים אישיים, פרויקטים, כישורים, צ'אט, קבצים ותמונות.

## דרישות מקדימות

- [.NET SDK 7](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Node.js](https://nodejs.org/) (מומלץ LTS) — ללקוח
- **SQL Server** (אופציונלי בפיתוח — ראו למטה)

## מבנה הפרויקט

| תיקייה | תיאור |
|--------|--------|
| שורש | Web API (`Program.cs`, `Controllers/`, `BLL/`, `Data/`) |
| `Client/` | אפליקציית React + Vite |

## הגדרת שרת (API)

### מחרוזת חיבור ו־JWT

ערוך את `appsettings.json` (או השתמש ב־`appsettings.Development.json` אם קיים):

- **ConnectionStrings:DefaultConnection** — מחרוזת חיבור ל־SQL Server.
- **Jwt** — חובה להגדיר `Issuer`, `Audience` ו־`Key` (מפתח סימטרי ארוך; בפרודקשן השתמש בסוד אקראי חזק).

אם ב־**Development** מחרוזת החיבור ריקה או תואמת את המכונה המקומית שמוגדרת בקוד (`DESKTOP-IV6MTF1\RUTHB`), השרת עובר אוטומטית ל־**InMemory database** — נוח להרצה מהירה בלי SQL Server.

### הרצת ה־API

```bash
cd /path/to/Microservice_Profile-main
dotnet run --launch-profile http
```

ברירת המחדל לפי `Properties/launchSettings.json`:

- HTTP: `http://localhost:5290`
- פרופיל `https`: גם `https://localhost:7182`

במצב **Development** זמינים **Swagger UI** ו־**OpenAPI** (בדרך כלל תחת `/swagger`).

### אימות (Auth)

- `POST /api/auth/register` — הרשמה
- `POST /api/auth/login` — התחברות
- `POST /api/auth/dev-token` — יצירת JWT לפיתוח בלבד (רק כש־`Development`)

שאר ה־Controllers ממופים תחת `api/...` (למשל `api/Users`, `api/Projects`, `api/Chat`, `api/Skills` וכו').

## הגדרת לקוח (React)

### משתני סביבה

העתק את `Client/.env.example` ל־`Client/.env` (או `.env.local`) והתאם:

- **VITE_API_BASE_URL** — חייב להסתיים ב־`/api` (למשל `http://127.0.0.1:5290/api`), אחרת בקשות האימות עלולות לפגוע בנתיב שגוי.
- אופציונלי: **VITE_DEV_PROXY_TARGET** — יעד לפרוקסי של Vite (ברירת מחדל `http://127.0.0.1:5290`) כשמשתמשים ב־`VITE_API_BASE_URL=/api`.

### הרצת הלקוח

```bash
cd Client
npm install
npm run dev
```

לבנייה לפרודקשן: `npm run build` — הפלט ב־`Client/dist` (השרת מגיש קבצים סטטיים דרך `UseStaticFiles` אם מוגדר בהתאם).

## אינטגרציות נוספות

ב־`appsettings.json` קיים קטע **Staff6Events** (נקודת קצה ו־API Key) לפרסום אירועים — יש להחליף ערכי placeholder בסביבה אמיתית.

## פיתוח מהיר (סיכום)

1. הגדר `Jwt:Key` ומחרוזת חיבור (או הסתמך על InMemory ב־Development).
2. `dotnet run --launch-profile http` בשורש הפרויקט.
3. ב־`Client/`: `npm install && npm run dev`, עם `VITE_API_BASE_URL` מצביע על `http://127.0.0.1:5290/api`.

---

פרויקט לימודי/דמו — לפני פריסה יש לחזק סודות JWT, CORS ומדיניות אבטחה בהתאם לסביבה.
