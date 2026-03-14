# 🏗️ Business Logic Layer (BLL) - Documentation

שכבת ה-BLL מהווה את הליבה הלוגית של המערכת. היא אחראית על עיבוד הנתונים, אכיפת חוקים עסקיים, וביצוע הפרדה מוחלטת בין שכבת ה-Data (Entities) לבין שכבת ה-Presentation (Controllers/DTOs).

---

## 🛠️ עקרונות ועבודה עם Clean Architecture
השכבה נבנתה תוך הקפדה על הפרדת רשויות (**Decoupling**):
* **Interfaces First:** עבודה מול ממשקים המאפשרת הזרקת תלות (Dependency Injection) ובדיקות יחידה קלות.
* **DTO Mapping:** שימוש ב-**AutoMapper** למניעת חשיפה של מודלים מה-DB החוצה.
* **Service Orchestration:** ריכוז לוגיקה מורכבת (כמו ניהול קשר Many-to-Many של Skills) בתוך שירותים ייעודיים.

---

## 📂 מבנה התיקיות

| תיקייה | תפקיד |
| :--- | :--- |
| **Interfaces** | הגדרת ה-Contracts עבור ה-Services וה-Repositories. |
| **Repositories** | מימוש הגישה לנתונים באמצעות LINQ ו-EF Core (כולל Generic Repository). |
| **Services** | מימוש הלוגיקה העסקית, ניהול קבצים ואימות נתונים. |
| **Mapping** | הגדרת פרופילי המיפוי של AutoMapper. |

---

## ⚙️ פיצ'רים מרכזיים שהוטמעו

### 1. ניהול פרופיל וכישורים (Skills)
* ניהול ישות `PersonalDetails` המקושרת ל-`User`.
* לוגיקת עדכון כישורים (Skills) המטפלת אוטומטית בטבלה המקשרת (`SkillToUser`) - הוספת כישורים חדשים והסרת ישנים בשיחת API אחת.

### 2. מערכת ניהול פרויקטים
* מימוש CRUD מלא עבור פרויקטים של סטודנטים.
* אינטגרציה עם `FileService` לשמירת תמונות פרויקט בשרת.

### 3. אימות נתונים (Validation)
הוספת שכבת הגנה ב-BLL לפני שמירה ב-DB:
* **Email:** אימות פורמט באמצעות Regex.
* **URLs:** בדיקת תקינות לינקים ל-GitHub ולינקים חיצוניים.
* **File Security:** הגבלת העלאת קבצים לסוגי תמונות בלבד ועד נפח של 5MB.

---

## 🔧 התקנה והרצה
השכבה דורשת את חבילת ה-NuGet הבאה (מותקנת ב-`.csproj`):
`AutoMapper.Extensions.Microsoft.DependencyInjection`

רישום השירותים ב-`Program.cs`:
```csharp
// Repositories & Services Registration
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
---

## 🔌 API Endpoints (חשיפה דרך ה-Controllers)

| Controller | Method | Endpoint | Description |
| :--- | :--- | :--- | :--- |
| **Student** | GET | `/api/student/{userId}` | שליפת פרופיל סטודנט מלא כולל Skills |
| **Student** | POST/PUT | `/api/student` | עדכון פרטים אישיים וניהול רשימת כישורים |
| **Project** | GET | `/api/project/user/{userId}` | שליפת כל הפרויקטים של משתמש ספציפי |
| **Project** | POST | `/api/project` | הוספת פרויקט חדש (כולל העלאת תמונה) |
| **Project** | DELETE | `/api/project/{id}` | מחיקת פרויקט קיים |
| **File** | POST | `/api/file/upload` | שירות עזר להעלאת קבצים עם ולידציית נפח |

---

## 👩‍💻 פותח על ידי
**נועה** - אחראית על שכבת ה- **BLL** (חלק 2)
