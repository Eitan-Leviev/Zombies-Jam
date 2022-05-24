# ex5-flocking-bar-eitan
ex5-flocking-bar-eitan created by GitHub Classroom

## פלוקינג  
ראשית מימשנו את שלושת הרכיבים המרכזיים של פלוקינג-  
separation, alignment, cohesion
והוספנו 2 פרמטרים נוספים
- "אגו" : self : פרמטר אחראי ל"אגו" של הדמות - כלומר החשיבות של הכיוון הנוכחי של הדמות לעומת גורמים אחרים.
- מנהיג : leader : מנגנון התקרבות מבוסס arrival, כדי שעוקבים יתקבצו סביב המנהיג.

כמו כן לא ממשנו separation  מה leader path - כפי שמתואר בleader following,  כדי ליצור תחושה של בלאגן, שהדמויות כן יכולות להפריע למנהיג. 

ל2 הקבוצות שלנו ניסינו ליצור התנהגות.הקבוצות שלנו מדמות "זומבים" ו"בני אדם":
- בני האדם מבצעים פלוקינג רגיל יחסית - משקלים מאוזנים, רדיוס בינוני, ומשקל self לא זניח.
כמו כן העוקבים מנסים להגיע מאחורי המנהיג ויש לו השפעה גדולה יחסית עליהם, ולכן קל לו יחסית לנווט אותם.
- לעומת זאת אצל הזומבים- על מנת ליצור התנהגות של תפיסת שטח גדול במרחב, ושהות בו, בלי צפיפות גדולה, נתנו משקל גבוה לseparation ומשקל נמוך מאוד לשאר הפרמטרים, מה שבשילוב עם רדיוס ראייה רחב יחסית, גורם להם ליצור צורה יציבה יחסית של כל זומבי באיזור משלו, אבל עדיין משתדלים להישאר כגוש משותף.
בנוסף העוקבים מנסים להגיע לאותו מקום של הלידר, מה שבצירוף עם ההתנהגות הקודמת - גורם לאפקט של ענן גדול של זומבים סביב המנהיג.  
- אינטרציה בין 2 הקבוצות:  
הזומבים משתמשים ב separation שלהם כדי לרדוף אחרי בני אדם, ובדומה- בני האדם בורחים מזומבים. השפעה נמוכה יותר של משקל המנהיג על הזומבים גם מאפשר להם בקלות יותר להינתק ממנו על מנת לרדוף.

שתי הקבוצות בעלות זווית ראייה לא מושלמת - מה שמאפשר למנהיגים "לנטוש" במקרה הצורך עוקבים, על ידי כניסה לאיזור מוסתר שלהם, או מאפשר לפעמים לעוקב לעזוב את המנהיג על דעת עצמו בקלות יותר.

## משחקיות  
ניסינו ליצור מעין תופסת בין זומבים לבני האדם. 
המטרה של מנהיג בני האדם היא להגיע לקצה הימני של המגרש, ה"איזור בטוח", לאחר שלא נשארו עוד בני אדם להציל - כלומר אחרון.
המטרה של הזומבים היא לתפוס מאת מנהיג בני האדם.
כל אדם שניתקל בזומבי - הופך לזומבי.
בני אדם המצליחים להגיע לקצה הימני של המגרש - ה"אזור בטוח", מצליחים לברוח ולא הופכים לזומבים.

##  מה היינו משפרים בהינתן עוד זמן
מוסיפים mesh שיהפוך את צורת הכובע למשהו מגניב ורנדומלי
היינו מרחיבים את העולם ויוצרים שלבים מגוונים יותר - אולי אפילו יצירת שלבים באופן פרוצדורלי
היינו מוסיפים עצמים לעולם כדי להעשיר את הניראות ואת המשחקיות כגון עצים, אבנים וכו
היינו מנסים קבוצה מול קבוצה של בני אדם כאשר הזומבים נשלטים ע"י פלוקינג גאשר המנהיג ע"י AI
היינו מייצרים התנהגויות יותר מורכבות כגון obstacle avoidance, pursuit, evasion
