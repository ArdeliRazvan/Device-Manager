/**
 * init-db.js — Script de inițializare și populare MongoDB
 *
 * IDEMPOTENT: Poate fi rulat de multiple ori fără a crea duplicate.
 * Verifică existența datelor înainte de inserare.
 *
 * Rulare:
 *   mongosh mongodb://localhost:27017 --file init-db.js
 *   sau: mongo mongodb://localhost:27017 --file init-db.js (versiuni vechi)
 *
 * Ce face:
 *   1. Creează baza de date DeviceManagerDB dacă nu există
 *   2. Creează colecțiile 'devices' și 'users' cu indecși unici
 *   3. Inserează date dummy pentru testare
 */


// 1. SELECTARE BAZĂ DE DATE

db = db.getSiblingDB("DeviceManagerDB");

print("=== Device Manager DB — Inițializare ===\n");


// 2. CREARE COLECȚII CU VALIDARE SCHEMA (opțional, pentru robustețe)


// Creăm colecția 'devices' dacă nu există deja
const existingCollections = db.getCollectionNames();

if (!existingCollections.includes("devices")) {
    db.createCollection("devices", {
        validator: {
            $jsonSchema: {
                bsonType: "object",
                required: ["name", "manufacturer", "type", "os", "osVersion", "processor", "ram"],
                properties: {
                    name: { bsonType: "string", description: "Numele dispozitivului — obligatoriu" },
                    manufacturer: { bsonType: "string" },
                    type: { enum: ["phone", "tablet"], description: "Trebuie să fie 'phone' sau 'tablet'" },
                    os: { bsonType: "string" },
                    osVersion: { bsonType: "string" },
                    processor: { bsonType: "string" },
                    ram: { bsonType: "int", minimum: 1, maximum: 64 }
                }
            }
        }
    });
    print("✓ Colecția 'devices' creată cu validare schema.");
} else {
    print("→ Colecția 'devices' există deja.");
}

if (!existingCollections.includes("users")) {
    db.createCollection("users");
    print("✓ Colecția 'users' creată.");
} else {
    print("→ Colecția 'users' există deja.");
}


// CREARE INDECȘI (idempotent — MongoDB ignoră dacă există)


db.devices.createIndex({ name: 1 }, { unique: true, name: "idx_device_name_unique" });
db.devices.createIndex(
    { name: "text", manufacturer: "text", processor: "text" },
    { name: "idx_device_text_search" }
);
db.users.createIndex({ email: 1 }, { unique: true, name: "idx_user_email_unique" });

print("✓ Indecși creați/verificați.");


// DATE DUMMY — UTILIZATORI


const usersToInsert = [
    {
        name: "Alexandru Popescu",
        role: "admin",
        location: "București, România",
        email: "alex.popescu@devicemanager.ro",
        // Parola: "Admin@1234" — în producție se folosește BCrypt din backend
        passwordHash: "$2a$12$examplehashforadmin000000000000000000000000000000000",
        createdAt: new Date()
    },
    {
        name: "Admin Demo",
        role: "admin",
        location: "Bucharest, Romania",
        email: "admin@demo.com",
        passwordHash: "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj0oHPOGJ5Hy",
        createdAt: new Date()
    },
    {
        name: "Maria Ionescu",
        role: "user",
        location: "Cluj-Napoca, România",
        email: "maria.ionescu@devicemanager.ro",
        passwordHash: "$2a$12$examplehashforuser0000000000000000000000000000000000",
        createdAt: new Date()
    },
    {
        name: "Andrei Constantin",
        role: "user",
        location: "Timișoara, România",
        email: "andrei.constantin@devicemanager.ro",
        passwordHash: "$2a$12$examplehashforuser1000000000000000000000000000000000",
        createdAt: new Date()
    }
];

let usersInserted = 0;
for (const user of usersToInsert) {
    const exists = db.users.findOne({ email: user.email });
    if (!exists) {
        db.users.insertOne(user);
        usersInserted++;
        print(`  + User inserat: ${user.email}`);
    } else {
        print(`  → User există deja: ${user.email}`);
    }
}
print(`✓ Utilizatori inserați: ${usersInserted}/${usersToInsert.length}\n`);

// Preluăm ID-urile utilizatorilor pentru a asigna dispozitive
const adminUser = db.users.findOne({ email: "alex.popescu@devicemanager.ro" });
const mariaUser = db.users.findOne({ email: "maria.ionescu@devicemanager.ro" });

//Dispozitivve

const devicesToInsert = [
    {
        name: "iPhone 15 Pro",
        manufacturer: "Apple",
        type: "phone",
        os: "iOS",
        osVersion: "17.4",
        processor: "Apple A17 Pro",
        ram: 8,
        description: "Flagship Apple cu chip A17 Pro, cameră de 48MP și Dynamic Island.",
        assignedUserId: adminUser ? adminUser._id : null,
        createdAt: new Date("2024-01-10T10:00:00Z"),
        updatedAt: new Date("2024-01-10T10:00:00Z")
    },
    {
        name: "Samsung Galaxy S24 Ultra",
        manufacturer: "Samsung",
        type: "phone",
        os: "Android",
        osVersion: "14",
        processor: "Snapdragon 8 Gen 3",
        ram: 12,
        description: "Telefon premium Samsung cu S Pen integrat și cameră de 200MP.",
        assignedUserId: mariaUser ? mariaUser._id : null,
        createdAt: new Date("2024-01-11T09:00:00Z"),
        updatedAt: new Date("2024-01-11T09:00:00Z")
    },
    {
        name: "Google Pixel 8",
        manufacturer: "Google",
        type: "phone",
        os: "Android",
        osVersion: "14",
        processor: "Google Tensor G3",
        ram: 8,
        description: "Telefon Google cu AI integrat, actualizări garantate 7 ani.",
        assignedUserId: null,
        createdAt: new Date("2024-01-12T11:00:00Z"),
        updatedAt: new Date("2024-01-12T11:00:00Z")
    },
    {
        name: "Samsung Galaxy Tab S9",
        manufacturer: "Samsung",
        type: "tablet",
        os: "Android",
        osVersion: "13",
        processor: "Snapdragon 8 Gen 2",
        ram: 8,
        description: "Tabletă premium cu display Dynamic AMOLED 2X și stylus S Pen inclus.",
        assignedUserId: null,
        createdAt: new Date("2024-01-13T08:00:00Z"),
        updatedAt: new Date("2024-01-13T08:00:00Z")
    },
    {
        name: "iPad Pro 12.9 M4",
        manufacturer: "Apple",
        type: "tablet",
        os: "iPadOS",
        osVersion: "17.4",
        processor: "Apple M4",
        ram: 16,
        description: "Cea mai puternică tabletă Apple cu chip M4 și display Ultra Retina XDR.",
        assignedUserId: null,
        createdAt: new Date("2024-01-14T09:30:00Z"),
        updatedAt: new Date("2024-01-14T09:30:00Z")
    },
    {
        name: "OnePlus 12",
        manufacturer: "OnePlus",
        type: "phone",
        os: "Android",
        osVersion: "14",
        processor: "Snapdragon 8 Gen 3",
        ram: 12,
        description: "Flagship killer cu încărcare rapidă de 100W și display LTPO 4.0.",
        assignedUserId: null,
        createdAt: new Date("2024-01-15T10:00:00Z"),
        updatedAt: new Date("2024-01-15T10:00:00Z")
    },
    {
        name: "Xiaomi 14 Ultra",
        manufacturer: "Xiaomi",
        type: "phone",
        os: "Android",
        osVersion: "14",
        processor: "Snapdragon 8 Gen 3",
        ram: 16,
        description: "Camera phone profesional cu lentile Leica și zoom optic 5x.",
        assignedUserId: null,
        createdAt: new Date("2024-01-16T07:00:00Z"),
        updatedAt: new Date("2024-01-16T07:00:00Z")
    },
    {
        name: "Microsoft Surface Pro 10",
        manufacturer: "Microsoft",
        type: "tablet",
        os: "Windows",
        osVersion: "11",
        processor: "Intel Core Ultra 7",
        ram: 16,
        description: "Tabletă 2-în-1 Windows cu Neural Processing Unit pentru AI local.",
        assignedUserId: null,
        createdAt: new Date("2024-01-17T08:00:00Z"),
        updatedAt: new Date("2024-01-17T08:00:00Z")
    }
];

let devicesInserted = 0;
for (const device of devicesToInsert) {
    const exists = db.devices.findOne({ name: device.name });
    if (!exists) {
        db.devices.insertOne(device);
        devicesInserted++;
        print(`  + Dispozitiv inserat: ${device.name}`);
    } else {
        print(`  → Dispozitiv există deja: ${device.name}`);
    }
}

print(`✓ Dispozitive inserare: ${devicesInserted}/${devicesToInsert.length}\n`);

// ============================================================
// 6. VERIFICARE FINALĂ
// ============================================================

print("=== SUMAR ===");
print(`Dispozitive în BD:  ${db.devices.countDocuments()}`);
print(`Utilizatori în BD:  ${db.users.countDocuments()}`);
print(`Dispozitive asignate: ${db.devices.countDocuments({ assignedUserId: { $ne: null } })}`);
print("\n✅ Inițializare completă!");