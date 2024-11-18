const express = require('express');
const router = express.Router();
const { db } = require('../config/firebase-config');

// Get all ejercicios
router.get('/', async (req, res) => {
    try {
        const ejerciciosSnapshot = await db.collection('ejercicios').get();
        const ejercicios = [];
        ejerciciosSnapshot.forEach(doc => {
            ejercicios.push({ id: doc.id, ...doc.data() });
        });
        res.json(ejercicios);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Get ejercicio by ID
router.get('/:id', async (req, res) => {
    try {
        const ejercicioDoc = await db.collection('ejercicios').doc(req.params.id).get();
        if (!ejercicioDoc.exists) {
            return res.status(404).json({ message: 'Ejercicio no encontrado' });
        }
        res.json({ id: ejercicioDoc.id, ...ejercicioDoc.data() });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Create ejercicio
router.post('/', async (req, res) => {
    try {
        const { id_ejercicio, ...ejercicioData } = req.body;
        await db.collection('ejercicios').doc(id_ejercicio).set(ejercicioData);
        res.status(201).json({ id: id_ejercicio, ...ejercicioData });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Update ejercicio
router.put('/:id', async (req, res) => {
    try {
        const ejercicioRef = db.collection('ejercicios').doc(req.params.id);
        const ejercicio = await ejercicioRef.get();
        if (!ejercicio.exists) {
            return res.status(404).json({ message: 'Ejercicio no encontrado' });
        }
        await ejercicioRef.update(req.body);
        res.json({ message: 'Ejercicio actualizado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Delete ejercicio
router.delete('/:id', async (req, res) => {
    try {
        const ejercicioRef = db.collection('ejercicios').doc(req.params.id);
        const ejercicio = await ejercicioRef.get();
        if (!ejercicio.exists) {
            return res.status(404).json({ message: 'Ejercicio no encontrado' });
        }
        await ejercicioRef.delete();
        res.json({ message: 'Ejercicio eliminado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

module.exports = router;
