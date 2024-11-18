const express = require('express');
const router = express.Router();
const { db } = require('../config/firebase-config');

// CRUD operations for GPS
router.get('/', async (req, res) => {
    try {
        const gpsSnapshot = await db.collection('gps').get();
        const gpsData = [];
        gpsSnapshot.forEach(doc => gpsData.push({ id: doc.id, ...doc.data() }));
        res.json(gpsData);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Get gps by ID
router.get('/:id', async (req, res) => {
    try {
        const gpsDoc = await db.collection('gps').doc(req.params.id).get();
        if (!gpsDoc.exists) {
            return res.status(404).json({ message: 'gps no encontrado' });
        }
        res.json({ id: gpsDoc.id, ...gpsDoc.data() });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Create gps
router.post('/', async (req, res) => {
    try {
        const { id_gps, ...gpsData } = req.body;
        await db.collection('gps').doc(id_gps).set(gpsData);
        res.status(201).json({ id: id_gps, ...gpsData });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Update gps
router.put('/:id', async (req, res) => {
    try {
        const gpsRef = db.collection('gps').doc(req.params.id);
        const gps = await gpsRef.get();
        if (!gps.exists) {
            return res.status(404).json({ message: 'gps no encontrado' });
        }
        await gpsRef.update(req.body);
        res.json({ message: 'gps actualizado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Delete gps
router.delete('/:id', async (req, res) => {
    try {
        const gpsRef = db.collection('gps').doc(req.params.id);
        const gps = await gpsRef.get();
        if (!gps.exists) {
            return res.status(404).json({ message: 'gps no encontrado' });
        }
        await gpsRef.delete();
        res.json({ message: 'gps eliminado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

module.exports = router;