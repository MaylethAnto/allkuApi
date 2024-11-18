const express = require('express');
const router = express.Router();
const { db } = require('../config/firebase-config');

// Get all caninos
router.get('/', async (req, res) => {
    try {
        const caninosSnapshot = await db.collection('caninos').get();
        const caninos = [];
        caninosSnapshot.forEach(doc => {
            caninos.push({ id: doc.id, ...doc.data() });
        });
        res.json(caninos);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Get canino by id
router.get('/:id', async (req, res) => {
    try {
        const caninoDoc = await db.collection('caninos').doc(req.params.id).get();
        if (!caninoDoc.exists) {
            return res.status(404).json({ message: 'Canino no encontrado' });
        }
        res.json({ id: caninoDoc.id, ...caninoDoc.data() });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Create canino
router.post('/', async (req, res) => {
    try {
        const { id_canino, ...caninoData } = req.body;
        await db.collection('caninos').doc(id_canino).set(caninoData);
        res.status(201).json({ id: id_canino, ...caninoData });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Update canino
router.put('/:id', async (req, res) => {
    try {
        const caninoRef = db.collection('caninos').doc(req.params.id);
        const canino = await caninoRef.get();
        if (!canino.exists) {
            return res.status(404).json({ message: 'Canino no encontrado' });
        }
        await caninoRef.update(req.body);
        res.json({ message: 'Canino actualizado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Delete canino
router.delete('/:id', async (req, res) => {
    try {
        const caninoRef = db.collection('caninos').doc(req.params.id);
        const canino = await caninoRef.get();
        if (!canino.exists) {
            return res.status(404).json({ message: 'Canino no encontrado' });
        }
        await caninoRef.delete();
        res.json({ message: 'Canino eliminado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

module.exports = router;
